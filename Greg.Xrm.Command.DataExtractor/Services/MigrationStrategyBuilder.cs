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

			while (tablesToMap.Count > 0)
			{
				var leafList = FindLeaves(tables);
				if (leafList.Count == 0)
				{
					// handle circular dependency
					throw new InvalidOperationException("Circular dependency detected");
					continue;
				}


				foreach (var leaf in leafList.OrderBy(x => x.Name))
				{
					tablesToMap.Remove(leaf);
					result.Add(leaf.Name);
				}
			}

			return result;
		}




		private static IReadOnlyList<TableModel> FindLeaves(IReadOnlyList<TableModel> tables) => tables.Where(x => x.IsLeaf).ToList();
	}
}
