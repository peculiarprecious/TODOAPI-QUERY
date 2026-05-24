namespace TODOAPI_QUERY.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public required Dictionary<string, string[]>? Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
