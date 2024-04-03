using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.DataExtractor
{
	[Command("data", "tree", "build", HelpText ="")]
	public class DataTreeBuildCommand
	{
		[Option("solution", "s", HelpText = "The name of the solution containing the entities to export. If not specified, the default solution is used instead.")]
		public string? SolutionName { get; set; }
	}
}
