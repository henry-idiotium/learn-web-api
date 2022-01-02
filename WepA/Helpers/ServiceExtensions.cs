using WepA.Services;
using WepA.Models.Domains;
using WepA.Interfaces.Services;
using WepA.Data;
using System.Text;
using System.Collections.Generic;
using System;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WepA.Interfaces.Repositories;
using WepA.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace WepA.Helpers
{
	public static class ServiceExtensions
	{
		public static void AddCorsExt(this IServiceCollection services)
		{
			services.AddCors(o => o.AddPolicy("DevelopmentPolicy", builder =>
				builder.AllowAnyMethod()
					.AllowAnyHeader()
					.WithOrigins(
						"https://localhost:5000",
						"https://localhost:5001")));
		}

		public static void AddIdentityExt(this IServiceCollection services, bool isDevelopment)
		{
			if (isDevelopment)
			{
				services.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					options.SignIn.RequireConfirmedEmail = false;
					options.SignIn.RequireConfirmedAccount = false;
					options.SignIn.RequireConfirmedPhoneNumber = false;
				})
				.AddEntityFrameworkStores<WepADbContext>()
				.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

				services.Configure<IdentityOptions>(options =>
				{
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 0;
					options.Password.RequiredUniqueChars = 0;
				});
			}
		}

		public static void AddMapsterExt(this IServiceCollection services)
		{
			var config = new TypeAdapterConfig();

			config.NewConfig<ApplicationUser, UserDetailsResponse>()
				  .Map(dest => dest.EncodedId, src => EncryptHelpers.EncodeBase64Url(src.Id));

			services.AddSingleton(config);
			services.AddScoped<IMapper, ServiceMapper>();
		}

		public static void AddSwaggerExt(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "WepA", Version = "v1" });
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using the Bearer scheme." +
						"\nEnter 'Bearer' [space] and then your token in the text input below." +
						"\nExample: 'Bearer 12345abcdef'",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,
						},
						new List<string>()
					}
				});
			});
		}

		public static void AddAuthenticationExt(this IServiceCollection services, string secret)
		{
			var key = Encoding.ASCII.GetBytes(secret);
			var validLocations = new List<string>{
				"https://localhost:5001",
				"https://localhost:5000"
			};
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = true;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,

					ValidIssuers = validLocations,
					ValidAudiences = validLocations,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ClockSkew = TimeSpan.FromMinutes(1)
				};
			});
		}

		public static void AddDIContainerExt(this IServiceCollection services)
		{
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IUserRepository, UserRepository>();
		}
	}
}