using WepA.Middlewares;
using WepA.Helpers.Settings;
using WepA.Helpers;
using WepA.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

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

			// service extensions
			services.AddIdentityExt(CurrentEnvironment.IsDevelopment());
			services.AddAutoMapper(typeof(Startup));
			services.AddCorsExt();
			services.AddDIContainerExt();

			// option-patterns
			services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
			services.Configure<SendGridSettings>(Configuration.GetSection("ExternalProviders:SendGrid"));

			services.AddControllers();
			services.AddSwaggerExt();
		}

		// Middlewares — This method gets called by the runtime. Use this method to configure the
		// HTTP request pipeline.
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

			app.UseHttpStatusExceptionHandling();
			app.UseJwtMiddleware();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
