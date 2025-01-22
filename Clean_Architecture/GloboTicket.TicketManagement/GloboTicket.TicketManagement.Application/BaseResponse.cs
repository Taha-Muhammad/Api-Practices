namespace GloboTicket.TicketManagement.Application
{
	public class BaseResponse
	{
		public BaseResponse()
		{
			Success = true;
		}
		public BaseResponse(string message)
		{
			Success = true;
			Message = message;
		}
		public BaseResponse(bool success, string message)
		{
			Success = success;
			Message = message;
		}

		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public List<string>? ValidationErrors { get; set; }
	}
}
