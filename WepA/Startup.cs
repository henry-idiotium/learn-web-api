using WepA.Services;
using WepA.Interfaces.Services;
using WepA.Extensions.Helpers;
using WepA.Extensions;
using WepA.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
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
			services.AddDbContext<WepADbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("WepA")));

			services.IdentityConfiguration(CurrentEnvironment.IsDevelopment());

			services.AuthenticationConfiguration(
				Configuration.GetSection("JwtSettings").GetSection("Secret").Value);

			services.CorsConfiguration();

			services.AddAutoMapper(typeof(Startup));

			// Provide specified key for JwtUtil class instance - Options pattern
			services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

			services.AddFluentEmail(Configuration.GetValue<string>("EmailSettings:FromMail"))
					.AddRazorRenderer(Directory.GetCurrentDirectory())
					.AddSmtpSender(new SmtpClient
					{
						Host = Configuration.GetValue<string>("EmailSettings:Host"),
						EnableSsl = Configuration.GetValue<bool>("EmailSettings:IsEnableSsl"),
						Port = Configuration.GetValue<int>("EmailSettings:Port"),
						DeliveryMethod = SmtpDeliveryMethod.Network
					});

			services.AddControllers();
			services.SwaggerConfiguration();
		}

		// This method runs after ConfigureServices, so the methods
		// here will override any registrations made in ConfigureServices.
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterType<UserService>().As<IUserService>();
			builder.RegisterType<AccountService>().As<IAccountService>();
			builder.RegisterType<JwtUtil>().As<IJwtUtil>();
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

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseJwtMiddleware();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
