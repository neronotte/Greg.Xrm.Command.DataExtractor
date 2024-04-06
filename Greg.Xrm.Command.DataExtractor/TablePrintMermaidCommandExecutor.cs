using Greg.Xrm.Command.DataExtractor.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.DataExtractor
{
	public class TablePrintMermaidCommandExecutor : ICommandExecutor<TablePrintMermaidCommand>
	{
		private readonly IOutput output;
		private readonly IOrganizationServiceRepository organizationServiceRepository;

		public TablePrintMermaidCommandExecutor(
			IOutput output,
			IOrganizationServiceRepository organizationServiceRepository)
		{
			this.output = output;
			this.organizationServiceRepository = organizationServiceRepository;
		}


		public async Task<CommandResult> ExecuteAsync(TablePrintMermaidCommand command, CancellationToken cancellationToken)
		{
			this.output.Write($"Connecting to the current dataverse environment...");
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync();
			this.output.WriteLine("Done", ConsoleColor.Green);


			var currentSolutionName = command.SolutionName;
			if (string.IsNullOrWhiteSpace(currentSolutionName))
			{
				currentSolutionName = await organizationServiceRepository.GetCurrentDefaultSolutionAsync();
				if (currentSolutionName == null)
				{
					return CommandResult.Fail("No solution name provided and no current solution name found in the settings. Please provide a solution name or set a current solution name in the settings.");
				}
			}






			this.output.Write($"Retrieving tables from solution '{currentSolutionName}'...");
			var query = new QueryExpression("solutioncomponent");
			query.ColumnSet.AddColumns("objectid");
			query.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 1);
			var solutionLink = query.AddLink("solution", "solutionid", "solutionid");
			solutionLink.LinkCriteria.AddCondition("uniquename", ConditionOperator.Equal, currentSolutionName);
			query.NoLock = true;

			var tableIds = (await crm.RetrieveMultipleAsync(query))
				.Entities
				.Select(x => x.GetAttributeValue<Guid>("objectid"))
				.Distinct()
				.ToList();

			this.output.WriteLine("Done", ConsoleColor.Green);

			this.output.WriteLine($"Found {tableIds.Count} tables in solution '{currentSolutionName}'");



			this.output.WriteLine("Retrieving tables metadata...");
			query = new QueryExpression("entity");
			query.ColumnSet.AddColumns("logicalname");
			query.Criteria.AddCondition("entityid", ConditionOperator.In, tableIds.Cast<object>().ToArray());
			query.NoLock = true;

			var tableNames = (await crm.RetrieveMultipleAsync(query))
				.Entities
				.Select(x => x.GetAttributeValue<string>("logicalname"))
				.Select(x => x.ToLowerInvariant())
				.ToArray();




			var tableGraphBuilder = new TableGraphBuilder(this.output);
			var (missingTables, graph) = await tableGraphBuilder.BuildGraphAsync(crm, tableNames, command.IncludeSecurityTables, command.SkipMissingTables);



			if (!command.SkipMissingTables && missingTables.HasMissingTables)
			{
				return CommandResult.Fail(missingTables.ToString());
			}


			var diagram = graph.ToMermaidDiagram();

			this.output.WriteLine(diagram, ConsoleColor.White);

			return CommandResult.Success();
		}
	}
}
