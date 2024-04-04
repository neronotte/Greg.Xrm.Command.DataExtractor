namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class LookupFieldModel
	{
		public LookupFieldModel(string columnName, string tableName)
		{
			if (string.IsNullOrWhiteSpace(columnName))
			{
				throw new ArgumentNullException(nameof(columnName), $"'{nameof(columnName)}' cannot be null or empty.");
			}

			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName), $"'{nameof(tableName)}' cannot be null or whitespace.");
			}

			ColumnName = columnName;
			TableName = tableName.ToLowerInvariant();
		}

		public string ColumnName { get; }
		public string TableName { get; }


		public override string ToString()
		{
			return $"{ColumnName} ({TableName})";
		}
	}

}
