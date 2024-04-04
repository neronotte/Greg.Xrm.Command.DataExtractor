﻿namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class Graph
	{
		private readonly Dictionary<string, Table> nodes;

		public Graph(IReadOnlyCollection<Table> nodes) : this(nodes.ToDictionary(n => n.Name))
		{
		}



		public Graph(Dictionary<string, Table> nodes)
		{
			this.nodes = nodes;
		}


		public IDictionary<string, Table> Nodes => nodes;





		public List<Cycle> FindAllCycles()
		{
			var visited = new HashSet<string>();
			var allCycles = new List<Cycle>();

			foreach (var node in nodes.Values)
			{
				var path = new List<Node>();
				FindAllCycles(node, visited, path, allCycles);
			}

			return allCycles.Distinct().ToList();
		}

		private void FindAllCycles(Table node, HashSet<string> visited, List<Node> path, List<Cycle> allCycles)
		{
			visited.Add(node.Name);

			foreach (var lookup in node.Fields)
			{
				var nextNodeName = lookup.TableName;
				path.Add(new Node(node.Name, lookup.Columns, lookup.TableName));

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
				else if ((pathElement = path.Find(x => x.FromTable == nextNodeName)) != null)
				{
					// Found a cycle
					var cycle = new Cycle(this);
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
}
