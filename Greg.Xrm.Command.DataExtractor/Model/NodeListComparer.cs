using System.Diagnostics.CodeAnalysis;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	class NodeListComparer : IEqualityComparer<List<Node>>
	{
		public bool Equals(List<Node>? x, List<Node>? y)
		{
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;

			return x.SequenceEqual(y);
		}

		public int GetHashCode([DisallowNull] List<Node> obj)
		{
			return 0;
		}
	}
}
