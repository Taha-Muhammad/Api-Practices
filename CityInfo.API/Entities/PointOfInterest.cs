﻿namespace CityInfo.API.Entities
{
	public class PointOfInterest
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public City? City { get; set; }
		public int CityId { get; set; }
		public PointOfInterest(string name)
		{
			Name = name;
		}
	}
}