using WepA.Services.Interfaces;
using WepA.Services;
using WepA.Models;
using WepA.Data.Repositories.Interfaces;
using WepA.Data.Repositories;
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

namespace WepA
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Configure DbContext
			services.AddDbContext<WepADbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("WepA")));

			// Identity settings for development
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedPhoneNumber = false;
			})
			.AddEntityFrameworkStores<WepADbContext>();

			// Identity configurations for development
			services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 0;
				options.Password.RequiredUniqueChars = 0;

				// Lockout settings.
				// options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
				// options.Lockout.MaxFailedAccessAttempts = 5;
				// options.Lockout.AllowedForNewUsers      = true;

				// User settings.
				// options.User.AllowedUserNameCharacters =
				// "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;
			});

			// AutoMapper Service
			services.AddAutoMapper(typeof(Startup));

			services.AddControllers();

			services.AddSwaggerGen(c =>
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "WepA", Version = "v1" }));
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

			app.UseAuthorization();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
