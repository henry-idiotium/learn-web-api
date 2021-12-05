using WepA.Services;
using WepA.Interfaces.Services;
using WepA.Extensions.Helpers;
using WepA.Extensions;
using WepA.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Autofac;
using WepA.Utils;
using WepA.Interfaces.Utils;

namespace WepA
{
	public class Startup
	{
		public Startup(IWebHostEnvironment currentEnvironment, IConfiguration configuration)
		{
			CurrentEnvironment = currentEnvironment;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		private IWebHostEnvironment CurrentEnvironment { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Configure DbContext
			services.AddDbContext<WepADbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("WepA")));

			services.IdentityConfiguration(CurrentEnvironment.IsDevelopment());

			services.AuthenticationConfiguration(
				Configuration.GetSection("JwtSettings").GetSection("Secret").Value);

			services.CorsConfiguration();

				// User settings.
				// options.User.AllowedUserNameCharacters =
				// "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;
			});

			// AutoMapper Service
			services.AddAutoMapper(typeof(Startup));

			// Provide specified key for JwtUtil class instance - Options pattern
			services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

			services.AddControllers();
			services.SwaggerConfiguration();
		}

		// This method runs after ConfigureServices, so the methods
		// here will override any registrations made in ConfigureServices.
		public void ConfigureContainer(ContainerBuilder builder)
		{
			// User manage register
			builder.RegisterType<UserService>().As<IUserService>();
			builder.RegisterType<UserRepository>().As<IUserRepository>();

			// Account manage
			builder.RegisterType<AccountService>().As<IAccountService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request
		// pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WepA v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("DevelopmentPolicy");

			app.UseAuthorization();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
