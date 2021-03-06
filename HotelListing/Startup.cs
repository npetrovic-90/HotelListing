using AspNetCoreRateLimit;
using AutoMapper;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HotelListing
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			//adding dbcontext as a service
			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"));
			});


			//memory cache for rate limit
			services.AddMemoryCache();

			services.ConfigureRateLimiting();
			services.AddHttpContextAccessor();

			//caching service and header
			services.ConfigureHttpCacheHeaders();

			//Identity for users 

			services.AddAuthentication();
			services.ConfigureIdentity();
			//JWT configuration
			services.ConfigureJWT(Configuration);

			//adding controllers
			services.AddControllers(config =>
			{
				//caching
				config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });


			}).AddNewtonsoftJson(option =>
			{
				//avoid loops
				option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			});

			//adding cors policy

			services.AddCors(o =>
			{
				o.AddPolicy("AllowAll", builder =>
				{
					builder.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader();
				});

			});
			//auto mapper service

			services.AddAutoMapper(typeof(MapperInitializer));

			//registering services for api
			services.AddScoped<IAuthManager, AuthManager>();
			services.AddTransient<IUnitOfWork, UnitOfWork>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
			});

			//api versioning

			services.ConfigureVersioning();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

			}
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

			app.ConfigureExceptionHandler();

			app.UseHttpsRedirection();

			//user cors
			app.UseCors("AllowAll");

			app.UseResponseCaching();
			app.UseHttpCacheHeaders();
			app.UseIpRateLimiting();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
