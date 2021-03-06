using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace HotelListing.Data
{
	public class DatabaseContext : IdentityDbContext<ApiUser>
	{
		public DatabaseContext(DbContextOptions options) : base(options)
		{

		}


		//dbset definitions
		public DbSet<Country> Countries { get; set; }
		public DbSet<Hotel> Hotels { get; set; }


		//seeding db
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new RoleConfiguration());

			builder.ApplyConfiguration(new CountryConfiguration());
			builder.ApplyConfiguration(new HotelConfiguration());





		}
	}
}
