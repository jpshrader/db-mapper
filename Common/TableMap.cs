using System.Collections.Generic;

namespace db_mapper.Common {
	public class TableMap {
		public string TableName { get; set; }

		public string TableSchema { get; set; }

		public List<ColumnMap> ColumnMaps { get; set; }
	}
}
