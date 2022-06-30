using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
	public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
	{
		public void Configure(EntityTypeBuilder<Hotel> builder)
		{
			builder.HasData(
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
