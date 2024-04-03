using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.DataExtractor
{
	[Command("data", "tree", "build", HelpText ="")]
	public class DataTreeBuildCommand
	{
		[Option("solution", "s", HelpText = "The name of the solution containing the entities to export.")]
		[Required]
		public string? SolutionName { get; set; }
	}
}
