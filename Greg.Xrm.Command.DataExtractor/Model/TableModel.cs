namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class TableModel
	{
		private List<LookupFieldModel> allFields;
		private List<LookupFieldModel> filteredFields;


		public static TableModel Create(string tableName, params string[] lookup)
		{
			if (lookup.Length % 2 != 0)
			{
				throw new ArgumentException("The 'lookup' parameter must contain an even number of elements.");
			}

			var fields = new List<LookupFieldModel>();
			for (int i = 0; i < lookup.Length; i += 2)
			{
				fields.Add(new LookupFieldModel(lookup[i], lookup[i+1]));
			}


			return new TableModel(tableName, fields);
		}


		public TableModel(string tableName, List<LookupFieldModel> fields)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName), $"'{nameof(tableName)}' cannot be null or whitespace.");
			}

			this.Name = tableName.ToLowerInvariant();
			this.allFields = fields ?? new List<LookupFieldModel>();
			this.filteredFields = new List<LookupFieldModel>(allFields);
		}



		public string Name { get; }

		public IReadOnlyList<LookupFieldModel> Fields => this.filteredFields;

		public bool IsLeaf => this.Fields.Count == 0;

		public void FilterFields(IReadOnlyCollection<string> entityNames)
		{
			this.filteredFields.RemoveAll(f => entityNames.Contains(f.TableName, StringComparer.OrdinalIgnoreCase));
		}
	}
}
