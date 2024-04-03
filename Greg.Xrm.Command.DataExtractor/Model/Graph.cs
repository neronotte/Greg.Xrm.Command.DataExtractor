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

		public List<List<Node>> FindAllCycles()
		{
			var visited = new HashSet<string>();
			var allCycles = new List<List<Node>>();

			foreach (var node in nodes.Values)
			{
				var path = new List<Node>();
				FindAllCycles(node, visited, path, allCycles);
			}

			return allCycles;
		}

		private void FindAllCycles(TableModel node, HashSet<string> visited, List<Node> path, List<List<Node>> allCycles)
		{
			visited.Add(node.Name);

			foreach (var lookup in node.Fields)
			{
				var nextNodeName = lookup.TableName;
				var columnName = node.Name + "." + lookup.ColumnName;
				path.Add(new Node(node.Name, lookup.ColumnName));

				if (!nodes.ContainsKey(nextNodeName))
				{
					continue;
				}

				var nextNode = nodes[nextNodeName];

				Node? pathElement;
				if (!visited.Contains(nextNodeName))
				{
					FindAllCycles(nextNode, visited, path, allCycles);
				}
				else if ((pathElement = path.Find(x => x.Table == nextNodeName)) != null)
				{
					// Found a cycle
					var cycle = new List<Node>();
					for (int i = path.IndexOf(pathElement); 
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

	public record Node(string Table, string Column);
}
