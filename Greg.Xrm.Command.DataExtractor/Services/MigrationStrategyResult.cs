namespace Greg.Xrm.Command.DataExtractor.Services
{
	public class MigrationStrategyResult
	{
		private MigrationStrategyResult(string errorMessage)
		{
			this.HasError = true;
			this.ErrorMessage = errorMessage;
		}


		public MigrationStrategyResult()
		{
			this.HasError = false;
			this.ErrorMessage = string.Empty;
		}

		public static MigrationStrategyResult Error(string errorMessage) => new MigrationStrategyResult(errorMessage);


		public bool HasError { get;  }
		public string ErrorMessage { get; }

		public List<string> TableNames { get; } = new List<string>();

		public MigrationStrategyResult Add(string tableName)
		{
			this.TableNames.Add(tableName);
			return this;
		}
	}
}
