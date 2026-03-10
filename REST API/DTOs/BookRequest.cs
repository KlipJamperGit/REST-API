using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs;

public class BookRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    [Range(1, 2100)]
    public int PublishedYear { get; set; }
}
