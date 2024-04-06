using Greg.Xrm.Command.DataExtractor.GraphManipulation;
using Greg.Xrm.Command.DataExtractor.Model;

namespace Greg.Xrm.Command.DataExtractor.TestSuite
{
	static class Extensions
	{
		public static DirectedGraph<TableModel> AddTable(this DirectedGraph<TableModel> graph, string name)
		{
			graph.AddNode(new TableModel(name));
			return graph;
		}

		public static DirectedGraph<TableModel> AddRelation(this DirectedGraph<TableModel> graph, string fromTable, string toTable, params string[] columnNames)
		{
			graph.AddArch(graph[fromTable], graph[toTable], new Dictionary<string, object> { { "columns", columnNames } });
			return graph;
		}
	}

}