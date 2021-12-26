using db_mapper.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web.UI;

namespace db_mapper {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddScoped<IAppSettingsProvider, AppSettingsProvider>();
			services.AddScoped<IDataAccessProvider, DataAccessProvider>();

			services.AddRazorPages()
				.AddMvcOptions(options => { })
				.AddMicrosoftIdentityUI();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseEndpoints(endpoints => {
				endpoints.MapRazorPages();
				endpoints.MapControllerRoute("Default",
					  "{controller}/{action}/{id?}",
					  new { controller = "App", Action = "Index" });
				endpoints.MapControllers();
			});
		}
	}
}
