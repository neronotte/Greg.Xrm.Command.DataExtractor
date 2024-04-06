using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Greg.Xrm.Command.DataExtractor.GraphManipulation
{
	/// <summary>
	/// A directed arc in a directed graph
	/// </summary>
	/// <typeparam name="INodeContent"></typeparam>
	[DebuggerDisplay("{From} -> {To}")]
	class DirectedArc<T> : IReadOnlyDictionary<string, object>, IDirectedArc<T>
		where T : INodeContent
	{
		private readonly Dictionary<string, object> additionalInfo;

		/// <summary>
		/// Creates a new arch from a node to another node
		/// </summary>
		/// <param name="from">The starting node of the arc</param>
		/// <param name="to">The final node of the arc</param>
		/// <param name="additionalInfo">Additional information about the arc</param>
		/// <exception cref="ArgumentNullException">If one of the from or to node is null</exception>
		public DirectedArc(DirectedNode<T> from, DirectedNode<T> to, IReadOnlyDictionary<string, object>? additionalInfo)
		{
			this.From = from ?? throw new ArgumentNullException(nameof(from));
			this.To = to ?? throw new ArgumentNullException(nameof(to));
			this.additionalInfo = additionalInfo != null ? new Dictionary<string, object>(additionalInfo) : new();
		}

		/// <summary>
		/// Gets the starting node of the arc
		/// </summary>
		public DirectedNode<T> From { get; }

		/// <summary>
		/// Gets the starting node of the arc
		/// </summary>
		IDirectedNode<T> IDirectedArc<T>.From => this.From;


		/// <summary>
		/// Gets the final node of the arc
		/// </summary>
		public DirectedNode<T> To { get; }

		/// <summary>
		/// Gets the final node of the arc
		/// </summary>
		IDirectedNode<T> IDirectedArc<T>.To => this.To;

		public ICollection<string> Keys => this.additionalInfo.Keys;

		public ICollection<object> Values => this.additionalInfo.Values;

		public int Count => this.additionalInfo.Count;

		public bool IsReadOnly => false;

		IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => ((IReadOnlyDictionary<string, object>)additionalInfo).Keys;

		IEnumerable<object> IReadOnlyDictionary<string, object>.Values => ((IReadOnlyDictionary<string, object>)additionalInfo).Values;


		public object this[string index]
		{
#pragma warning disable CS8603 // Possible null reference return.
			get => GetAdditionalInfo<object>(index);
#pragma warning restore CS8603 // Possible null reference return.
			set => SetAdditionalInfo(index, value);
		}

		public T1? GetAdditionalInfo<T1>(string key)
		{
			if (additionalInfo.TryGetValue(key, out var value))
			{
				return (T1)value;
			}
			return default;
		}

		public void SetAdditionalInfo(string key, object? value)
		{
			if (value == null)
			{
				additionalInfo.Remove(key);
				return;
			}

			additionalInfo[key] = value;
		}



		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				return From.GetHashCode() ^ To.GetHashCode();
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is not DirectedArc<T> other) return false;
			return From.Equals(other.From) && To.Equals(other.To);
		}

		public void Add(string key, object value)
		{
			SetAdditionalInfo(key, value);
		}

		public bool ContainsKey(string key)
		{
			return this.additionalInfo.ContainsKey(key);
		}


		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this.additionalInfo.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
		{
			return this.additionalInfo.TryGetValue(key, out value);
		}
	}
}
