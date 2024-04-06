using System.Text;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class MissingTableCache
	{
		private readonly Dictionary<string, List<string>> cache = new();

		public void Add(string tableName, string referencedByName)
		{
			if (!this.cache.ContainsKey(tableName))
			{
				this.cache[tableName] = new List<string>();
			}

			if (this.cache[tableName].Contains(referencedByName))
			{
				return;
			}

			this.cache[tableName].Add(referencedByName);
		}


		public bool HasMissingTables => this.cache.Count > 0;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("The following tables are missing from the list:");
			foreach (var item in this.cache)
			{
				sb.AppendLine($"  - {item.Key} is referenced by {string.Join(", ", item.Value.Order())}");
			}

			return sb.ToString();
		}
	}
}
