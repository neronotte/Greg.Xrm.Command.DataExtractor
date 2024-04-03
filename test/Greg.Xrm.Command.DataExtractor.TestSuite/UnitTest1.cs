using Greg.Xrm.Command.DataExtractor.Model;

namespace Greg.Xrm.Command.DataExtractor.TestSuite
{
	[TestClass]
	public class GraphTests
	{
		[TestMethod]
		public void TestFindAllCycles_NoCycles()
		{
			var nodes = new []
			{
				TableModel.Create("a", "bid", "b"),
				TableModel.Create("b", "cid", "c"),
				TableModel.Create("c")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(0, cycles.Count);
		}

		[TestMethod]
		public void TestFindAllCycles_OneCycle()
		{
			var nodes = new[]
			{
				TableModel.Create("a", "bid", "b"),
				TableModel.Create("b", "cid", "c"),
				TableModel.Create("c", "aid", "a")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid"), new("b", "cid"), new("c", "aid") }, cycles[0]);
		}

		[TestMethod]
		public void TestFindAllCycles_OneCycle_2()
		{
			var nodes = new[]
			{
				TableModel.Create("a", "bid", "b", "did", "d"),
				TableModel.Create("b", "cid", "c"),
				TableModel.Create("c", "aid", "a"),
				TableModel.Create("d", "eid", "e")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid"), new("b", "cid"), new("c", "aid") }, cycles[0]);
		}

		[TestMethod]
		public void TestFindAllCycles_MultipleCycles()
		{
			var nodes = new[]
			{
				TableModel.Create("a", "bid", "b", "cid", "c"),
				TableModel.Create("b", "aid", "a"),
				TableModel.Create("c", "aid", "a")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(2, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid"), new("b", "aid") }, cycles[0]);
			CollectionAssert.AreEquivalent(new List<Node> { new ("a", "cid"), new("c", "aid") }, cycles[1]);
		}
	}
}