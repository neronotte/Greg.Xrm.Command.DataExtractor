using Greg.Xrm.Command.DataExtractor.GraphManipulation;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class TableModel : INodeContent
	{
		private readonly string name;

		public TableModel(string name)
        {
			this.name = name.ToLowerInvariant();
		}

		public object Key => this.name;


		public override string ToString()
		{
			return this.name;
		}
	}
}
