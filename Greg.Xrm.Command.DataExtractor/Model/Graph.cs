namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class Graph
	{
		private readonly Dictionary<string, TableModel> nodes;

		public Graph(IReadOnlyCollection<TableModel> nodes)
		{
			this.nodes = nodes.ToDictionary(n => n.Name);
		}

		public Graph(Dictionary<string, TableModel> nodes)
		{
			this.nodes = nodes;
		}

		public List<List<string>> FindAllCycles()
		{
			var visited = new HashSet<string>();
			var allCycles = new List<List<string>>();

			foreach (var node in nodes.Values)
			{
				var path = new List<string>();
				FindAllCycles(node, visited, path, allCycles);
			}

			return allCycles;
		}

		private void FindAllCycles(TableModel node, HashSet<string> visited, List<string> path, List<List<string>> allCycles)
		{
			visited.Add(node.Name);

			foreach (var lookup in node.Fields)
			{
				var nextNodeName = lookup.TableName;
				var columnName = node.Name + "." + lookup.ColumnName;
				path.Add(columnName);

				if (!nodes.ContainsKey(nextNodeName))
				{
					continue;
				}

				var nextNode = nodes[nextNodeName];

				if (!visited.Contains(nextNodeName))
				{
					FindAllCycles(nextNode, visited, path, allCycles);
				}
				else if (path.Find(x => x.StartsWith(nextNodeName + ".")) != null)
				{
					// Found a cycle
					var cycle = new List<string>();
					for (int i = path.IndexOf(path.Find(x => x.StartsWith(nextNodeName + ".")) ?? string.Empty); 
						i < path.Count; 
						i++)
					{
						cycle.Add(path[i]);
					}
					allCycles.Add(cycle);
				}

				path.RemoveAt(path.Count - 1);
			}

		}
	}
}
