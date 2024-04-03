using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.DataExtractor.Model
{
	public class TableModel
	{
		private List<LookupFieldModel> allFields;
		private List<LookupFieldModel> filteredFields;

		public TableModel(string tableName, List<LookupFieldModel> fields)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName), $"'{nameof(tableName)}' cannot be null or whitespace.");
			}

			this.Name = tableName.ToLowerInvariant();
			this.allFields = fields ?? new List<LookupFieldModel>();
			this.filteredFields = new List<LookupFieldModel>(allFields);
		}



		public string Name { get; }

		public IReadOnlyList<LookupFieldModel> Fields => this.filteredFields;

		public bool IsLeaf => this.Fields.Count == 0;

		public void FilterFields(IReadOnlyCollection<string> entityNames)
		{
			this.filteredFields.RemoveAll(f => entityNames.Contains(f.TableName, StringComparer.OrdinalIgnoreCase));
		}
	}
}
