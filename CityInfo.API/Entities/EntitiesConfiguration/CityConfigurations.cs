using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityInfo.API.Entities.EntitiesConfiguration
{
	public class CityConfiguration : IEntityTypeConfiguration<City>
	{
		public void Configure(EntityTypeBuilder<City> builder)
		{
			builder.Property(c=>c.Name).IsRequired().HasMaxLength(50);
			builder.Property(c=>c.Description).HasMaxLength(200);
			builder.HasData(
				new City("New York City")
				{
					Id = 1,
					Description = "The one with that big park."
				},
				new City("Antwerp")
				{
					Id = 2,
					Description = "The one with the cathedral that was never really finished."
				}, new City("Paris")
				{
					Id = 3,
					Description = "The one with that big tower."
				});
		}
	}
}
