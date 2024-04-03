namespace Greg.Xrm.Command.DataExtractor.Services
{
	public class MigrationActionUpdateTableColumn : IMigrationAction
	{
		public MigrationActionUpdateTableColumn(string tableName, string columnName)
		{
			if (string.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
			}

			if (string.IsNullOrEmpty(columnName))
			{
				throw new ArgumentException($"'{nameof(columnName)}' cannot be null or empty.", nameof(columnName));
			}

			TableName = tableName;
			ColumnName = columnName;
		}

		public string TableName { get; }

		public string ColumnName { get; }


		public override string ToString()
		{
			return $"Update table <{TableName}> to set column <{ColumnName}>";
		}
	}
}
