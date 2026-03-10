namespace REST_API.Models;

public class Review
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
