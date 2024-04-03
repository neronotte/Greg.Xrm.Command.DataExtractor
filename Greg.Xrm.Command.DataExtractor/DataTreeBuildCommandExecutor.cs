using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.DataExtractor
{
	public class DataTreeBuildCommandExecutor : ICommandExecutor<DataTreeBuildCommand>
	{
		public Task<CommandResult> ExecuteAsync(DataTreeBuildCommand command, CancellationToken cancellationToken)
		{
		}
	}
}
