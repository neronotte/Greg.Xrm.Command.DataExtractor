using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	[DebuggerDisplay("{FromTable} --> {ToTable}")]
	public class Node : IEqualityComparer<Node>
	{
		public Node(string fromTable, string[] columns, string toTable)
		{
			FromTable = fromTable;
			Columns = columns;
			ToTable = toTable;
		}
		public Node(string fromTable, string column, string toTable)
		{
			FromTable = fromTable;
			Columns = new[] { column };
			ToTable = toTable;
		}

		public string FromTable { get; set; }
		public string[] Columns { get; set; }
		public string ToTable { get; set; }


		public override bool Equals(object? obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is not Node y) return false;

			var x = this;
			return x.FromTable == y.FromTable
				&& x.ToTable == y.ToTable
				&& x.Columns.Length == y.Columns.Length
				&& x.Columns.SequenceEqual(y.Columns);
		}

		public override int GetHashCode()
		{
			return this.FromTable.GetHashCode() ^ this.ToTable.GetHashCode();
		}


		public bool Equals(Node? x, Node? y)
		{
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;

			return x.Equals(y);
		}

		public int GetHashCode([DisallowNull] Node obj)
		{
			return obj.GetHashCode();
		}


		public override string ToString()
		{
			return $"{FromTable} --> {ToTable}";
		}
	}
}
