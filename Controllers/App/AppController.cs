using db_mapper.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace db_mapper.Controllers.App {
	public class AppController : Controller {
		private readonly IAppSettingsProvider appSettingsProvider;
		private readonly IDataAccessProvider dataAcessProvider;

		public AppController(IAppSettingsProvider appSettingsProvider, IDataAccessProvider dataAcessProvider) {
			this.appSettingsProvider = appSettingsProvider;
			this.dataAcessProvider = dataAcessProvider;
		}

		[HttpGet]
		public IActionResult Index() {
			var connectionOptions = GetConnectionOptions();
			var model = new AppViewModel {
				ConnectionInfo = connectionOptions
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult Index(AppViewModel model) {
			if (!ModelState.IsValid) {
				model.ConnectionInfo = GetConnectionOptions();
				ModelState.AddModelError(nameof(model.SelectedEnvironmentName), "Environment is Required");
				return View(model);
			}

			return RedirectToAction(nameof(ViewDb), new { environmentName = model.SelectedEnvironmentName, tableNameFilter = string.Empty });
		}

		[HttpGet]
		public IActionResult ViewDb(string environmentName, string tableNameFilter) {
			var connectionInfo = GetConnectionOptions().SingleOrDefault(o => o.EnvironmentName == environmentName);
			if (connectionInfo == null) {
				return NotFound();
			}
			var dataAccess = dataAcessProvider.GetDataAccess(connectionInfo);

			var map = dataAccess.GetMap();
			if (!string.IsNullOrWhiteSpace(tableNameFilter)) {
				map.TableMaps = map.TableMaps.Where(t => t.TableName.Contains(tableNameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
			}

			var model = new DbViewModel {
				EnvironmentName = environmentName,
				TableNameFilter = tableNameFilter,
				DatabaseMap = map
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult ViewDb(DbViewModel model) {
			return RedirectToAction(nameof(ViewDb), new { environmentName = model.EnvironmentName, tableNameFilter = model.TableNameFilter });
		}

		private IEnumerable<ConnectionInfo> GetConnectionOptions() {
			return appSettingsProvider.GetObject<IEnumerable<ConnectionInfo>>("ConnectionInfo");
		}
	}
}
