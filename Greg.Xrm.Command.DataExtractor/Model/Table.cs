namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class Table
	{
		private readonly List<AggregatedLookupFieldModel> allFields;
		private readonly List<AggregatedLookupFieldModel> filteredFields;


		public static Table Create(string tableName, params string[] lookup)
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


			return new Table(tableName, fields);
		}


		public Table(string tableName, List<LookupFieldModel> fields)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName), $"'{nameof(tableName)}' cannot be null or whitespace.");
			}

			this.Name = tableName.ToLowerInvariant();

			var aggregatedFields = fields
				.ToLookup(x => x.TableName)
				.Select(x => new AggregatedLookupFieldModel(x.ToArray()))
				.ToList();


			this.allFields = aggregatedFields;
			this.filteredFields = new List<AggregatedLookupFieldModel>(this.allFields);
		}



		public string Name { get; }

		public IReadOnlyList<AggregatedLookupFieldModel> OriginalFields => this.allFields;

		public IReadOnlyList<AggregatedLookupFieldModel> Fields => this.filteredFields;

		public bool IsLeaf => this.Fields.Count == 0;


		public bool HasAutoCycle
		{
			get 
			{
				return this.AutoCycle != null;
			}
		}

		public AggregatedLookupFieldModel? AutoCycle
		{
			get => this.Fields.FirstOrDefault(x => x.TableName == this.Name);
		}

		public void RemoveLookupsTowardsTables(IReadOnlyCollection<string> relatedTableNames)
		{
			this.filteredFields.RemoveAll(f => relatedTableNames.Contains(f.TableName, StringComparer.OrdinalIgnoreCase));
		}


		public override string ToString()
		{
			if (this.Fields.Count == 0)
			{
				return this.Name;
			}

			var dependencyTables = string.Join(", ", this.Fields.Select(x => x.TableName).Distinct().Order());

			return $"{this.Name} (depends on: {dependencyTables})";
		}
	}
}
