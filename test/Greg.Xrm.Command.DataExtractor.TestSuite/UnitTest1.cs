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
				Table.Create("a", "bid", "b"),
				Table.Create("b", "cid", "c"),
				Table.Create("c")
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
				Table.Create("a", "bid", "b"),
				Table.Create("b", "cid", "c"),
				Table.Create("c", "aid", "a")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid", "b"), new("b", "cid", "c"), new("c", "aid", "a") }, cycles[0]);
		}

		[TestMethod]
		public void TestFindAllCycles_OneCycle_2()
		{
			var nodes = new[]
			{
				Table.Create("a", "bid", "b", "did", "d"),
				Table.Create("b", "cid", "c"),
				Table.Create("c", "aid", "a"),
				Table.Create("d", "eid", "e")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(1, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid", "b"), new("b", "cid", "c"), new("c", "aid", "a") }, cycles[0]);
		}

		[TestMethod]
		public void TestFindAllCycles_MultipleCycles()
		{
			var nodes = new[]
			{
				Table.Create("a", "bid", "b", "cid", "c"),
				Table.Create("b", "aid", "a"),
				Table.Create("c", "aid", "a")
			};

			var graph = new Graph(nodes);
			var cycles = graph.FindAllCycles();

			Assert.AreEqual(2, cycles.Count);
			CollectionAssert.AreEquivalent(new List<Node> { new("a", "bid", "b"), new("b", "aid", "a") }, cycles[0]);
			CollectionAssert.AreEquivalent(new List<Node> { new ("a", "cid", "c"), new("c", "aid", "a") }, cycles[1]);
		}
	}
}