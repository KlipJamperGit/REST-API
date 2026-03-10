using Microsoft.AspNetCore.Mvc;
using REST_API.DTOs;
using REST_API.Models;
using REST_API.Services;

namespace REST_API.Controllers;

[ApiController]
[Route("api/books/{bookId}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IBookService _bookService;

    public ReviewsController(IReviewService reviewService, IBookService bookService)
    {
        _reviewService = reviewService;
        _bookService = bookService;
    }

    [HttpGet("/api/reviews")]
    public ActionResult<IEnumerable<Review>> GetAll()
    {
        return Ok(_reviewService.GetAll());
    }

    [HttpGet]
    public ActionResult<IEnumerable<Review>> GetAllForBook(int bookId)
    {
        if (_bookService.GetById(bookId) is null) return NotFound();
        return Ok(_reviewService.GetAllForBook(bookId));
    }

    [HttpGet("{id}")]
    public ActionResult<Review> GetById(int bookId, int id)
    {
        if (_bookService.GetById(bookId) is null) return NotFound();
        var review = _reviewService.GetById(bookId, id);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost]
    public ActionResult<Review> Create(int bookId, ReviewRequest request)
    {
        if (_bookService.GetById(bookId) is null) return NotFound();
        var review = new Review
        {
            ReviewerName = request.ReviewerName,
            Rating = request.Rating,
            Comment = request.Comment
        };
        var created = _reviewService.Add(bookId, review);
        return CreatedAtAction(nameof(GetById), new { bookId, id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Review> Update(int bookId, int id, ReviewRequest request)
    {
        if (_bookService.GetById(bookId) is null) return NotFound();
        var review = new Review
        {
            ReviewerName = request.ReviewerName,
            Rating = request.Rating,
            Comment = request.Comment
        };
        var updated = _reviewService.Update(bookId, id, review);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int bookId, int id)
    {
        if (_bookService.GetById(bookId) is null) return NotFound();
        return _reviewService.Delete(bookId, id) ? NoContent() : NotFound();
    }
}
