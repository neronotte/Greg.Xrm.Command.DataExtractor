using Greg.Xrm.Command.DataExtractor.Model;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.CompilerServices;
using System.Text;

namespace Greg.Xrm.Command.DataExtractor.Services
{
	public static class MigrationStrategyBuilder
	{
		public static MigrationStrategyResult Build(IReadOnlyList<Table> tables)
		{
			// find the leaf tables
			// if there is no leaf table, but we have then we have a circular dependency

			var result = new MigrationStrategyResult();
			var tablesToMap = new List<Table>(tables);

			var iterationCount = 0;
			while (tablesToMap.Count > 0 && !result.HasError)
			{
				result.MigrationActions.Add(new MigrationActionLog($"*** Iteration {++iterationCount} ***"));

				var leafList = FindLeaves(tablesToMap);
				if (leafList.Count > 0)
				{
					foreach (var leaf in leafList.OrderBy(x => x.Name))
					{
						result.Add(leaf.Name);
						tablesToMap.Remove(leaf);
					}

					foreach (var table in tablesToMap)
					{
						table.RemoveLookupsTowardsTables(leafList.Select(x => x.Name).ToList());
					}
				}
				else
				{
					// handle circular dependency
					var succeeded = TryManageCycles(tablesToMap, result);
					if (!succeeded)
					{
						return result;
					}
				}
			}

			return result;
		}

		public static bool TryValidateRelationships(IReadOnlyList<Table> tables, out string errorMessage)
		{
			var result = new Dictionary<string, HashSet<string>>();


			foreach (var table in tables)
			{
				foreach (var referencedTable in table.Fields.Select(x => x.TableName))
				{
					if (tables.Any(x => string.Equals(x.Name, referencedTable, StringComparison.OrdinalIgnoreCase)))
						continue;

					// la tabella non è presente nella lista
					if (!result.TryGetValue(referencedTable, out HashSet<string>? value))
					{
						value = new HashSet<string>();
						result[referencedTable] = value;
					}

					value.Add(table.Name);
				}
			}

			if (result.Count > 0)
			{
				var sb = new StringBuilder();
				sb.AppendLine("The following tables are missing from the list:");
				foreach (var item in result)
				{
					sb.AppendLine($"  - {item.Key} is referenced by {string.Join(", ", item.Value.Order())}");
				}

				errorMessage = sb.ToString();
				return false;
			}


			errorMessage = string.Empty;
			return true;
		}


		private static IReadOnlyList<Table> FindLeaves(IReadOnlyList<Table> tables) => tables.Where(x => x.IsLeaf).ToList();


		private static bool TryManageCycles(List<Table> tables, MigrationStrategyResult result)
		{
			var graph = new Graph(tables);

			var cycles = graph.FindAllCycles();
			if (cycles.Count == 0)
			{
				result.SetError($"The list contains {tables.Count} tables, but no cycles have been found, and no leafs are present. Please check with your admin.");
				return false;
			}

			if (cycles.Count == 1)
			{
				return TryBreakCycle(tables, cycles[0], result);
			}

			var autoCyclesSelfContained = cycles.Where(x => x.IsAutoCycle && x.IsSelfContained).ToList();
			if (autoCyclesSelfContained.Count > 0)
			{
				foreach (var cycle in autoCyclesSelfContained)
				{
					if (!TryBreakCycle(tables, cycle, result))
						return false;
				}

				return true;
			}

			var selfContainedCycles = cycles.Where(x => x.IsSelfContained).ToList();
			if (selfContainedCycles.Count > 0)
			{
				foreach (var cycle in selfContainedCycles)
				{
					if (!TryBreakCycle(tables, cycle, result))
						return false;
				}

				return true;
			}



			var sb = new StringBuilder();
			sb.AppendLine($"The list contains still {tables.Count} tables, but {cycles.Count} cycle{(cycles.Count > 1 ? "s have" : "has")} been found that we don't know how to manage: ");
			foreach (var cycle in cycles)
			{
				sb.AppendLine($"  - {string.Join(", ", cycle.Select(x => x.ToString()))}");
			}
			sb.AppendLine("You need to identify a manual solution to work with those cycles.");

			result.SetError(sb.ToString());
			return false;
		}




		private static bool TryBreakCycle(List<Table> tables, Cycle loop, MigrationStrategyResult result)
		{
			if (loop.IsAutoCycle)
			{
				var item = loop[0];
				var itemColumns = item.Columns
					.Select(x => $"{x} ({item.ToTable})")
					.ToArray();

				result.Add(new MigrationActionTableWithoutColumn(item.FromTable, string.Join(", ", itemColumns)));
				result.Add(new MigrationActionUpdateTableColumn(item.FromTable, string.Join(", ", itemColumns)));

				tables.Remove(tables.First(x => string.Equals(x.Name, item.FromTable, StringComparison.OrdinalIgnoreCase)));
				foreach (var table in tables)
				{
					table.RemoveLookupsTowardsTables(new[] { item.FromTable });
				}

				return true;
			}



			IMigrationAction? lastMigrationAction = null;
			for (int i = 0; i < loop.Count; i++)
			{
				var item = loop[i];

				var tableToImport = tables.Find(x => string.Equals(x.Name, item.FromTable, StringComparison.OrdinalIgnoreCase));
				if (tableToImport == null)
				{
					result.SetError($"Table {item.FromTable} not found in the list of tables.");
					return false;
				}

				var itemColumns = item.Columns
					.Select(x => $"{x} ({item.ToTable})")
					.ToArray();


				if (i == 0)
				{
					if (tableToImport.HasAutoCycle)
					{
						var columns1 = (tableToImport.AutoCycle?.Columns ?? Array.Empty<string>())
							.Select(x => $"{x} ({tableToImport.AutoCycle?.TableName})")
								.ToArray();
						var columns2 = columns1
							.Union(itemColumns)
							.ToArray();

						result.Add(new MigrationActionTableWithoutColumn(item.FromTable, string.Join(", ", columns2)));
						result.Add(new MigrationActionUpdateTableColumn(item.FromTable, string.Join(", ", columns1)));
						lastMigrationAction = new MigrationActionUpdateTableColumn(item.FromTable, string.Join(", ", itemColumns));
					}
					else
					{
						result.Add(new MigrationActionTableWithoutColumn(item.FromTable, string.Join(", ", itemColumns)));
						lastMigrationAction = new MigrationActionUpdateTableColumn(item.FromTable, string.Join(", ", itemColumns));
					}
				}
				else
				{
					if (tableToImport.HasAutoCycle)
					{
						var columns1 = (tableToImport.AutoCycle?.Columns ?? Array.Empty<string>())
							.Select(x => $"{x} ({tableToImport.AutoCycle?.TableName})")
								.ToArray();

						result.Add(new MigrationActionTableWithoutColumn(item.FromTable, string.Join(", ", columns1)));
						result.Add(new MigrationActionUpdateTableColumn(item.FromTable, string.Join(", ", columns1)));
					}
					else
					{
						result.Add(item.FromTable);
					}
				}

				tables.Remove(tables.First(x => string.Equals(x.Name, item.FromTable, StringComparison.OrdinalIgnoreCase)));
				foreach (var table in tables)
				{
					table.RemoveLookupsTowardsTables(new[] { item.FromTable });
				}
			}
			if (lastMigrationAction != null)
				result.Add(lastMigrationAction);

			return true;
		}

		internal static void RemoveMissingDependencies(List<Table> tables)
		{
			var tableDict = tables.ToDictionary(x => x.Name);
			foreach (var table in tables)
			{
				var tablesNotPresent = new List<string>();
				foreach (var relatedTable in table.Fields.Select(x => x.TableName))
				{
					if (!tableDict.ContainsKey(relatedTable))
					{
						tablesNotPresent.Add(relatedTable);
					}
				}

				table.RemoveLookupsTowardsTables(tablesNotPresent, true);
			}
		}
	}

}
