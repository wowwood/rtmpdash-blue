using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RTMPDash.Backend; 

public class Startup {
	public Startup(IConfiguration configuration) => Configuration = configuration;

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services) {
		services.AddRazorPages();
		services.AddSession(options => {
			options.IdleTimeout        = TimeSpan.MaxValue;
			options.Cookie.HttpOnly    = true;
			options.Cookie.IsEssential = true;
		});

		#if (DEBUG)
		services.AddControllers().AddRazorRuntimeCompilation();
		services.AddStackExchangeRedisCache(options => {
			options.Configuration = "localhost";
			options.InstanceName  = "RTMPdash_development";
		});
		#else
			services.AddControllers();
			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = "localhost";
				options.InstanceName = "RTMPdash_production";
			});
		#endif
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		if (env.IsDevelopment()) {
			app.UseDeveloperExceptionPage();
		}
		else {
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseStaticFiles();

		app.UseSession();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints => {
			endpoints.MapRazorPages();
			endpoints.MapControllers();
		});
	}
}
