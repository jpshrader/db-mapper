using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace db_mapper.Common {
	public class AppSettingsProvider : IAppSettingsProvider {
		private readonly IConfigurationRoot configuration;

		public AppSettingsProvider(string file = "appSettings.json") {
			var filePath = Path.Combine(Environment.CurrentDirectory, file);

			configuration = new ConfigurationBuilder()
				.AddJsonFile(filePath)
				.Build();
		}

		public string GetValue(string key) {
			return configuration[key];
		}

		public int GetInt(string key) {
			return int.Parse(GetValue(key));
		}

		public double GetDouble(string key) {
			return double.Parse(GetValue(key));
		}

		public string GetConnectionString(string key = "Default") {
			return configuration.GetConnectionString(key);
		}

		public T GetObject<T>(string key) {
			return configuration.GetSection(key).Get<T>();
		}
	}
}
