namespace Greg.Xrm.Command.DataExtractor.Services
{
	public class MigrationActionTableWithoutColumn : IMigrationAction
	{
		public MigrationActionTableWithoutColumn(string tableName, string columnName)
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
			return $"Import table <{TableName}> without column(s) <{ColumnName}>";
		}
	}
}
