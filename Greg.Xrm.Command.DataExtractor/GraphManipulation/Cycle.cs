using Greg.Xrm.Command.DataExtractor.Model;
using System.Collections;

namespace Greg.Xrm.Command.DataExtractor.GraphManipulation
{
	public class Cycle<T> : IReadOnlyList<IDirectedArc<T>>
		where T : INodeContent
	{
		private readonly List<IDirectedArc<T>> arcs = new();


		public Cycle(IEnumerable<IDirectedArc<T>> nodes)
		{
			this.arcs.AddRange(nodes);
		}


        #region IReadOnlyList<DirectedNode<T>>

        public IDirectedArc<T> this[int index] => this.arcs[index];

		public int Count => this.arcs.Count;

		public IEnumerator<IDirectedArc<T>> GetEnumerator()
		{
			return this.arcs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IEquality

		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				return this.arcs.Aggregate(0, (current, item) => current ^ (item != null ? item.GetHashCode() : 0)) ^ this.arcs.Count;
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is not Cycle<T> other) return false;

			return this.arcs.SequenceEqual(other.arcs);
		}

		#endregion



		/// <summary>
		/// Indicates whether the Cycle is a self-loop on a given node.
		/// </summary>
		public bool IsAutoCycle
		{
			get => this.arcs.Count == 1 && this.arcs[0].From.Equals(this.arcs[0].To);
		}


		/// <summary>
		/// Indicates whether the Cycle presents only references to nodes that are part of the cycle.
		/// </summary>
		public bool IsSelfContained
		{
			get
			{
				var nodesInCicle = this.arcs.Select(x => x.From).ToHashSet();

				foreach (var node in nodesInCicle)
				{
					foreach (var referencedNode in node.OutboundArcs.Select(x => x.To))
					{
						if (!nodesInCicle.Contains(referencedNode))
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
				var nodesInCicle = this.arcs.Select(x => x.From).ToHashSet();

				foreach (var node in nodesInCicle)
				{
					if (node.HasAutoCycle) return true;
				}

				return false;
			}
		}
	}
}
