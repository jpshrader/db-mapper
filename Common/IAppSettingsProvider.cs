namespace db_mapper.Common {
	public interface IAppSettingsProvider {
		string GetValue(string key);

		int GetInt(string key);

		double GetDouble(string key);

		string GetConnectionString(string key = "Default");

		T GetObject<T>(string key);
	}
}
