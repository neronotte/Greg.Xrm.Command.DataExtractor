namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class AggregatedLookupFieldModel
	{
		public AggregatedLookupFieldModel(IReadOnlyList<LookupFieldModel> lookups)
		{
			if (lookups == null || lookups.Count == 0)
				throw new ArgumentNullException(nameof(lookups), $"'{nameof(lookups)}' cannot be null or empty.");

			this.TableName = lookups[0].TableName;
			this.Columns = lookups.Select(x => x.ColumnName).Distinct().ToArray();
		}


		public string TableName { get; }
		public string[] Columns { get; }
	}

}
