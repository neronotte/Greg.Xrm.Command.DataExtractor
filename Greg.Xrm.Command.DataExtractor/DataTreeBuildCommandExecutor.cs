using Greg.Xrm.Command.DataExtractor.Model;
using Greg.Xrm.Command.DataExtractor.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

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
				.ToArray();


			if (!command.IncludeSecurityTables)
			{
				tableNames = tableNames.Except(SecurityTables.SecurityTableNames).ToArray();
			}

			var tables = new List<Table>();
			foreach (var tableName in tableNames)
			{
				var metadata = ((RetrieveEntityResponse)(await crm.ExecuteAsync(new RetrieveEntityRequest
				{
					LogicalName = tableName,
					EntityFilters = EntityFilters.Attributes
				}))).EntityMetadata;

				var lookupList = metadata.Attributes.OfType<LookupAttributeMetadata>()
					.SelectMany( x => x.Targets.Select(t => new LookupFieldModel(x.LogicalName, t)))
					.Where( x => command.IncludeSecurityTables || !SecurityTables.SecurityTableNames.Contains(x.TableName))
					.ToList();

				var table = new Table(metadata.LogicalName, lookupList);
				tables.Add(table);
			}

			this.output.WriteLine("Done", ConsoleColor.Green);

			this.output.WriteLine($"Found {tables.Count} table metadata in solution '{currentSolutionName}':");
			foreach (var table in tables)
			{
				this.output.WriteLine($"  - {table.Name}");
			}
			this.output.WriteLine();






			if (command.SkipMissingTables)
			{
				MigrationStrategyBuilder.RemoveMissingDependencies(tables);
			}
			else
			{
				if (!MigrationStrategyBuilder.TryValidateRelationships(tables, out var errorMessage))
				{
					return CommandResult.Fail(errorMessage);
				}
			}

			


			this.output.WriteLine("Building the data tree...");

			





			var result = MigrationStrategyBuilder.Build(tables);

			if (result.MigrationActions.Count > 0)
			{
				this.output.WriteLine($"The migration strategy is:");
				foreach (var migrationAction in result.MigrationActions)
				{
					this.output.WriteLine($"  - {migrationAction}");
				}
			}

			if (result.HasError)
			{
				return CommandResult.Fail(result.ErrorMessage);
			}



			return CommandResult.Success();
		}
	}
}
