namespace TODOAPI_QUERY.DTOs
{
    public class TodoResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "";
    }
}
