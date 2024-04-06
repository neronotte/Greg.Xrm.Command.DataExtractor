
namespace Greg.Xrm.Command.DataExtractor.GraphManipulation
{
	/// <summary>
	/// A node in a directed graph
	/// </summary>
	/// <typeparam name="INodeContent"></typeparam>
	class DirectedNode<T> : IDirectedNode<T>
		where T : INodeContent
	{
		public DirectedNode(T content)
		{
			Content = content ?? throw new ArgumentNullException(nameof(content));
		}

		/// <summary>
		/// Gets the content of the node
		/// </summary>
		public T Content { get; }

		/// <summary>
		/// Gets or sets the arcs that end into this node
		/// </summary>
		public List<DirectedArc<T>> InboundArcs { get; } = new List<DirectedArc<T>>();

		/// <summary>
		/// Gets the arcs that end into this node
		/// </summary>
		IReadOnlyList<IDirectedArc<T>> IDirectedNode<T>.InboundArcs => this.InboundArcs;

		/// <summary>
		/// Gets or sets the arcs that start from this node
		/// </summary>
		public List<DirectedArc<T>> OutboundArcs { get; } = new List<DirectedArc<T>>();

		/// <summary>
		/// Gets the arcs that start from this node
		/// </summary>
		IReadOnlyList<IDirectedArc<T>> IDirectedNode<T>.OutboundArcs => this.OutboundArcs;

		/// <summary>
		/// Indicates whether the node contains a self-loop
		/// </summary>
		public bool HasAutoCycle
		{
			get => this.OutboundArcs.Exists(x => x.To.Equals(this));
		}

		/// <summary>
		/// Returns the autocycle, if any
		/// </summary>
		public DirectedArc<T>? AutoCycle
		{
			get => this.OutboundArcs.Find(x => x.To.Equals(this));
		}

		/// <summary>
		/// Returns the autocycle, if any
		/// </summary>
		IDirectedArc<T>? IDirectedNode<T>.AutoCycle => this.AutoCycle;






		public override int GetHashCode()
		{
			return Content.Key.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is not DirectedNode<T> other) return false;
			return Content.Key.Equals(other.Content.Key);
		}


		public override string ToString()
		{
			return this.Content.ToString() ?? string.Empty;
		}
	}
}
