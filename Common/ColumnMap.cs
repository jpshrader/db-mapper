using System.Collections.Generic;

namespace db_mapper.Common {
	public class ColumnMap {
		public string ColumnName { get; set; }

		public string DataType { get; set; }

		public string DefaultValue { get; set; }

		public string IsPrimaryKey { get; set; }

		public string MaxLength { get; set; }

		public string IsNullable { get; set; }

		public string Description { get; set; }

		public List<ColumnRelationship> ColumnRelationships { get; set; }
	}
}
