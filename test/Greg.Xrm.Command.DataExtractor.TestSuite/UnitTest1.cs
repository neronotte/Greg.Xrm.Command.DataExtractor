using Greg.Xrm.Command.DataExtractor.GraphManipulation;
using Greg.Xrm.Command.DataExtractor.Model;

namespace Greg.Xrm.Command.DataExtractor.TestSuite
{
	[TestClass]
	public class GraphTests
	{
		[TestMethod]
		public void TestFindAllCycles_NoCycles()
		{
			var graph = new DirectedGraph<TableModel>();
			graph.AddTable("a");
			graph.AddTable("b");
			graph.AddTable("c");
			graph.AddRelation("a", "b", "bid");
			graph.AddRelation("b", "c", "cid");


			var cycles = graph.FindAllCycles();

			Assert.AreEqual(0, cycles.Count);
		}

		[TestMethod]
		public void TestFindAllCycles_OneCycle()
		{
			var graph = new DirectedGraph<TableModel>();
			graph.AddTable("a");
			graph.AddTable("b");
			graph.AddTable("c");
			graph.AddRelation("a", "b", "bid");
			graph.AddRelation("b", "c", "cid");
			graph.AddRelation("c", "a", "aid");

			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);
			var cycle = cycles[0];
			Assert.AreEqual("a", cycle[0].From.Content.Key);
			Assert.AreEqual("b", cycle[0].To.Content.Key);
			Assert.AreEqual("b", cycle[1].From.Content.Key);
			Assert.AreEqual("c", cycle[1].To.Content.Key);
			Assert.AreEqual("c", cycle[2].From.Content.Key);
			Assert.AreEqual("a", cycle[2].To.Content.Key);
		}




		[TestMethod]
		public void TestFindAllCycles_OneCycle_2()
		{
			var graph = new DirectedGraph<TableModel>();
			graph.AddTable("a");
			graph.AddTable("b");
			graph.AddTable("c");
			graph.AddTable("d");
			graph.AddTable("e");
			graph.AddRelation("a", "b", "bid");
			graph.AddRelation("a", "d", "did");
			graph.AddRelation("b", "c", "cid");
			graph.AddRelation("c", "a", "aid");
			graph.AddRelation("d", "e", "eid");

			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);

			var cycle = cycles[0];
			Assert.AreEqual("a", cycle[0].From.Content.Key);
			Assert.AreEqual("b", cycle[0].To.Content.Key);
			Assert.AreEqual("b", cycle[1].From.Content.Key);
			Assert.AreEqual("c", cycle[1].To.Content.Key);
			Assert.AreEqual("c", cycle[2].From.Content.Key);
			Assert.AreEqual("a", cycle[2].To.Content.Key);
		}




		[TestMethod]
		public void TestFindAllCycles_MultipleCycles()
		{
			var graph = new DirectedGraph<TableModel>();
			graph.AddTable("a");
			graph.AddTable("b");
			graph.AddTable("c");
			graph.AddRelation("a", "b", "bid");
			graph.AddRelation("a", "c", "cid");
			graph.AddRelation("b", "a", "aid");
			graph.AddRelation("c", "a", "aid");


			var cycles = graph.FindAllCycles();

			Assert.AreEqual(2, cycles.Count);


			Assert.AreEqual(2, cycles.Count);

			var cycle = cycles[0];
			Assert.AreEqual("a", cycle[0].From.Content.Key);
			Assert.AreEqual("b", cycle[0].To.Content.Key);
			Assert.AreEqual("b", cycle[1].From.Content.Key);
			Assert.AreEqual("a", cycle[1].To.Content.Key);

			cycle = cycles[1];
			Assert.AreEqual("a", cycle[0].From.Content.Key);
			Assert.AreEqual("c", cycle[0].To.Content.Key);
			Assert.AreEqual("c", cycle[1].From.Content.Key);
			Assert.AreEqual("a", cycle[1].To.Content.Key);
		}
	}

}