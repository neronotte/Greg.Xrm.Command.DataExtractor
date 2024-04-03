using Greg.Xrm.Command.DataExtractor.Model;
using System.Linq;

namespace Greg.Xrm.Command.DataExtractor.Services
{
	public class MigrationStrategyBuilder
	{
		public MigrationStrategyResult Build(IReadOnlyList<TableModel> tables)
		{
			// first of all, check that there is no relationship with tables not belonging to the list
			// if there is, then the solution must be enriched with the other tables

			var currentListTableNames = tables.Select(x => x.Name).ToList();
			var relatedTableNames = tables.SelectMany(x => x.Fields.Select(f => f.TableName)).Distinct().ToList();

			var missingTables = relatedTableNames.Except(currentListTableNames).ToList();
			if (missingTables.Count > 0)
			{
				return MigrationStrategyResult.Error($"The solution is missing the following tables: {string.Join(", ", missingTables)}");
			}


			// find the leaf tables
			// if there is no leaf table, but we have then we have a circular dependency

			var result = new MigrationStrategyResult();
			var tablesToMap = new List<TableModel>(tables);

			while (tablesToMap.Count > 0 && !result.HasError)
			{
				var leafList = FindLeaves(tables);
				if (leafList.Count > 0)
				{
					foreach (var leaf in leafList.OrderBy(x => x.Name))
					{
						result.Add(leaf.Name);
						tablesToMap.Remove(leaf);
					}

					foreach (var table in tablesToMap)
					{
						table.FilterFields(leafList.Select(x => x.Name).ToList());
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




		private static IReadOnlyList<TableModel> FindLeaves(IReadOnlyList<TableModel> tables) => tables.Where(x => x.IsLeaf).ToList();


		private static bool TryManageCycles(List<TableModel> tables, MigrationStrategyResult result)
		{
			var graph = new Graph(tables);

			var cycles = graph.FindAllCycles();
			if (cycles.Count == 0)
			{
				result.SetError( $"The list contains {tables.Count} tables, but no cycles have been found, and no leafs are present. Please check with your admin.");
				return false;
			}

			if (cycles.Count == 1)
			{
				return TryBreakCycle(tables, cycles[0], result);
			}

			// TODO: handle multiple cycles

			result.SetError($"The list contains {tables.Count} tables, but multiple cycles have been found. Please check with your admin.");
			return false;
		}

		private static bool TryBreakCycle(List<TableModel> tables, List<Node> loop, MigrationStrategyResult result)
		{
			IMigrationAction? lastMigrationAction = null;
			for (int i = 0; i < loop.Count; i++)
			{
				var item = loop[i];

				if (i == 0)
				{
					result.Add(new MigrationActionTableWithoutColumn(item.Table, item.Column));
					lastMigrationAction = new MigrationActionUpdateTableColumn(item.Table, item.Column);
				}
				else
				{
					result.Add(item.Table);
                }

				tables.Remove(tables.First(x => string.Equals(x.Name, item.Table, StringComparison.OrdinalIgnoreCase)));
				foreach (var table in tables)
				{
					table.FilterFields(new[] { item.Table });
				}
			}
			if (lastMigrationAction != null)
				result.Add(lastMigrationAction);

			return true;
		}
	}

}
