namespace TasksApi.Models
{
    public class TaskItem
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string? Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
