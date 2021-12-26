using System.Collections.Generic;

namespace db_mapper.Common {
	public class DatabaseMap {
		public string DbDescriptionName { get; set; }

		public string DbDescription { get; set; }

		public List<TableMap> TableMaps { get; set; }
	}
}
