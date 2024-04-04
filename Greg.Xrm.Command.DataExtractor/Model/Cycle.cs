using System.Collections;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class Cycle : IReadOnlyList<Node>, ICollection
	{
		private readonly List<Node> nodes = new();
		private readonly Graph graph;

		public Cycle(Graph graph)
		{
			this.graph = graph;
		}

		#region IReadOnlyList<Node>
		public Node this[int index] => this.nodes[index];

		public int Count => this.nodes.Count;

		public bool IsSynchronized => ((ICollection)nodes).IsSynchronized;

		public object SyncRoot => ((ICollection)nodes).SyncRoot;

		public IEnumerator<Node> GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		#region ICollection

		public void CopyTo(Array array, int index)
		{
			((ICollection)nodes).CopyTo(array, index);
		}

		#endregion


		#region IEquality

		public override int GetHashCode()
		{
			return this.nodes.Count;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is not Cycle other) return false;

			return this.nodes.SequenceEqual(other.nodes);
		}

		#endregion




		public void Add(Node node)
		{
			this.nodes.Add(node);
		}

		/// <summary>
		/// Indicates whether the Cycle is a self-loop on a given entity.
		/// </summary>
		public bool IsAutoCycle
		{
			get
			{
				return this.nodes.Count == 1 && this.nodes[0].FromTable == this.nodes[0].ToTable;
			}
		}


		/// <summary>
		/// Indicates whether the Cycle presents only references to nodes that are part of the cycle.
		/// </summary>
		public bool IsSelfContained
		{
			get 
			{
				var tablesInCycle = this.nodes.Select(x => x.FromTable).ToHashSet();

				foreach (var tableName in tablesInCycle)
				{
					if (!graph.Nodes.TryGetValue(tableName, out var table))
						throw new InvalidOperationException($"The node <{tableName}> is not part of the graph!");

					foreach (var referencedTableName  in table.Fields.Select(x => x.TableName))
					{
						if (!tablesInCycle.Contains(referencedTableName))
							return false;
					}
				}

				return true;
			}
		}


		/// <summary>
		/// Indicates whether any of the tables in the cycle contains a self-loop.
		/// </summary>
		public bool ContainsAutoCycle
		{
			get
			{
				var tablesInCycle = this.nodes.Select(x => x.FromTable).ToHashSet();

				foreach (var tableName in tablesInCycle)
				{
					if (!graph.Nodes.TryGetValue(tableName, out var table))
						throw new InvalidOperationException($"The node <{tableName}> is not part of the graph!");

					if (table.HasAutoCycle) return true;
				}

				return false;
			}
		}
	}
}
