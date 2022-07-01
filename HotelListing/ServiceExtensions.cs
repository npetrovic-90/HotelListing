﻿
using AspNetCoreRateLimit;
using HotelListing.Data;
using HotelListing.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;

namespace HotelListing
{
	public static class ServiceExtensions
	{
		public static void ConfigureIdentity(this IServiceCollection services)
		{
			var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);

			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
			builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
		}
		public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("Jwt");
			var key = Environment.GetEnvironmentVariable("KEY");


			services.AddAuthentication(option =>
			{
				option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings.GetSection("Issuer").Value,
					IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
				};


			});
		}

		public static void ConfigureExceptionHandler(this IApplicationBuilder app)
		{
			app.UseExceptionHandler(error =>
			{
				error.Run(async context =>
				{
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					context.Response.ContentType = "application/json";

					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

					if (contextFeature != null)
					{
						Log.Error($"Something went wrong in the {contextFeature.Error}");

						await context.Response.WriteAsync(new Error
						{
							StatusCode = context.Response.StatusCode,
							Message = "Internal Server Error.Please Try Again Later."
						}.ToString());
					}
				});
			});
		}

		public static void ConfigureVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(option =>
			{
				option.ReportApiVersions = true;
				option.AssumeDefaultVersionWhenUnspecified = true;
				option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
				option.ApiVersionReader = new HeaderApiVersionReader("api-version");
			});
		}

		public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
		{
			services.AddResponseCaching();
			services.AddHttpCacheHeaders((expirationOption =>
			{
				expirationOption.MaxAge = 120;
				expirationOption.CacheLocation = CacheLocation.Private;

			}), (validationOption) =>
			{
				validationOption.MustRevalidate = true;
			});
		}

		public static void ConfigureRateLimiting(this IServiceCollection services)
		{
			var rateLimitRules = new List<RateLimitRule>
			{
				new RateLimitRule
				{
					Endpoint="*",
					Limit=1,
					Period="5s"
				}
			};

			services.Configure<IpRateLimitOptions>(option =>
			{

				option.GeneralRules = rateLimitRules;
			});

			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

		}

	}
}
