using System;
using System.Collections.Generic;

namespace db_mapper.Common {
	public interface IDataAccessProvider {
		DataAccess GetDataAccess(ConnectionInfo connectionInfo);
	}

	public class DataAccessProvider : IDataAccessProvider {
		public DataAccess GetDataAccess(ConnectionInfo connectionInfo) {
			return connectionInfo.DbType switch {
				DbType.Sql => new SqlDataAccess(connectionInfo.ConnectionString),
				DbType.Postgres => throw new NotImplementedException(),
				_ => throw new KeyNotFoundException(),
			};
		}
	}
}
