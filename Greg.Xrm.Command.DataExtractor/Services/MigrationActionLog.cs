namespace Greg.Xrm.Command.DataExtractor.Services
{
	internal class MigrationActionLog : IMigrationAction
	{
		private readonly string message;

		public MigrationActionLog(string message)
        {
			this.message = message;
		}


		public override string ToString()
		{
			return this.message;
		}
	}
}
