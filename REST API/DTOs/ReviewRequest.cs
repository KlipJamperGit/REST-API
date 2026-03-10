using System.ComponentModel.DataAnnotations;

namespace REST_API.DTOs;

public class ReviewRequest
{
    [Required]
    public string ReviewerName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
