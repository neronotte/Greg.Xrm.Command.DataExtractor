using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.DataExtractor
{
	public class DataTreeBuildCommandExecutor : ICommandExecutor<DataTreeBuildCommand>
	{
		private readonly IOutput output;
		private readonly IOrganizationServiceRepository organizationServiceRepository;

		public DataTreeBuildCommandExecutor(
			IOutput output,
			IOrganizationServiceRepository organizationServiceRepository)
		{
			this.output = output;
			this.organizationServiceRepository = organizationServiceRepository;
		}


		public async Task<CommandResult> ExecuteAsync(DataTreeBuildCommand command, CancellationToken cancellationToken)
		{
			this.output.Write($"Connecting to the current dataverse environment...");
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync();
			this.output.WriteLine("Done", ConsoleColor.Green);



			





			return CommandResult.Success();
		}
	}
}
