namespace CourseLibrary.API.ResourceParameters
{
	public abstract class BaseResourceParameters
	{
		private const int maxPageSize = 20;
		private int _pageSize = 10;
		public virtual string? SearchQuery { get; set; }
		public virtual int PageNumber { get; set; } = 1;
		public virtual int PageSize
		{
			get => _pageSize; set => _pageSize
				= (value > maxPageSize) ? maxPageSize : value;
		}
	}

}
