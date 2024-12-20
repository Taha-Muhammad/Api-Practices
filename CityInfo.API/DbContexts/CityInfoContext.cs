using CityInfo.API.Entities;
using CityInfo.API.Entities.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
	public class CityInfoContext : DbContext
	{
		public DbSet<City> Cities { get; set; }
		public DbSet<PointOfInterest> PointsOfInterest { get; set; }
		private readonly IConfiguration _configuration;

		public CityInfoContext(IConfiguration configuration)
        {
			_configuration = configuration;
		}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_configuration.GetConnectionString("CityInfoConnectionString"));
			base.OnConfiguring(optionsBuilder);
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(CityConfiguration).Assembly);
			base.OnModelCreating(modelBuilder);
		}
	}
}
