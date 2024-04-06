
namespace Greg.Xrm.Command.DataExtractor.Services
{
	public class MigrationStrategyResult
	{
		private MigrationStrategyResult(string errorMessage)
		{
			this.ErrorMessage = errorMessage;
		}


		public MigrationStrategyResult()
		{
			this.ErrorMessage = string.Empty;
		}

		public static MigrationStrategyResult Error(string errorMessage) => new MigrationStrategyResult(errorMessage);


		public bool HasError => !string.IsNullOrWhiteSpace(this.ErrorMessage);
		public string ErrorMessage { get; private set; }

		public List<IMigrationAction> MigrationActions { get; } = new List<IMigrationAction>();

		public MigrationStrategyResult Add(string? tableName)
		{
			return this.Add(new MigrationActionFullTable(tableName));
		}

		public MigrationStrategyResult Add(IMigrationAction migrationAction)
		{
			this.MigrationActions.Add(migrationAction);
			return this;
		}

		internal void SetError(string errorMessage)
		{
			this.ErrorMessage = errorMessage;
		}
	}
}
