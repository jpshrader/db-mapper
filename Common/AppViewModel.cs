using System.Collections.Generic;

namespace db_mapper.Common {
	public class AppViewModel {
		public IEnumerable<ConnectionInfo> ConnectionInfo { get; set; }

		public string SelectedEnvironmentName { get; set; }
	}
}
