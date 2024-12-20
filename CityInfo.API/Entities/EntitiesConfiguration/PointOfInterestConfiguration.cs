using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityInfo.API.Entities.EntitiesConfiguration
{
	public class PointOfInterestConfiguration : IEntityTypeConfiguration<PointOfInterest>
	{
		public void Configure(EntityTypeBuilder<PointOfInterest> builder)
		{
			builder.Property(p=>p.Name).IsRequired().HasMaxLength(50);
			builder.Property(p=>p.Description).HasMaxLength(200);
			builder.HasData(
				new PointOfInterest("Central Park")
				{
					Id = 1,
					CityId = 1,
					Description = "The most visited urban park in the United States."
				},
				new PointOfInterest("Empire State Building")
				{
					Id = 2,
					CityId = 1,
					Description = "A 102-story skyscraper located in Midtown Manhattan."
				}, new PointOfInterest("Cathedral")
				{
					Id = 3,
					CityId = 2,
					Description = "A Gothic style cathedral, conceived by architects Jan."
				}, new PointOfInterest("Antwerp Central Station")
				{
					Id = 4,
					CityId = 2,
					Description = "The finest example of railway architecture in Belgium."
				}, new PointOfInterest("Eiffel Tower")
				{
					Id = 5,
					CityId = 3,
					Description = "A wrought iron lattice tower on the Champ de Mars."
				}, new PointOfInterest("The Louvre")
				{
					Id = 6,
					CityId = 3,
					Description = "The world's largest museum."
				});
		}
	}
	
}
