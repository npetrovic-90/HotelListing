using Microsoft.EntityFrameworkCore;
namespace HotelListing.Data
{
	public class DatabaseContext : DbContext
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

			builder.Entity<Country>().HasData(
				new Country
				{
					Id = 1,
					Name = "Serbia",
					ShortName = "SRB"
				},
				new Country
				{
					Id = 2,
					Name = "United States of America",
					ShortName = "USA"
				},
				new Country
				{
					Id = 3,
					Name = "Russia",
					ShortName = "RUS"
				}
				);

			builder.Entity<Hotel>().HasData(
				new Hotel
				{
					Id = 1,
					Name = "Sandals Resort and Spa",
					Address = "Autumn Street 1",
					CountryId = 1,
					Rating = 4.5

				},
				new Hotel
				{
					Id = 2,
					Name = "Four Seasons",
					Address = "New York Street 123",
					CountryId = 3,
					Rating = 3.0
				},
				new Hotel
				{
					Id = 3,
					Name = "Vodka Resort",
					Address = "Vodka Street 23",
					CountryId = 3,
					Rating = 5.0
				}
				);

		}
	}
}
