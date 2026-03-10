using Microsoft.AspNetCore.Mvc;
using REST_API.DTOs;
using REST_API.Models;
using REST_API.Services;

namespace REST_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IReviewService _reviewService;

    public BooksController(IBookService bookService, IReviewService reviewService)
    {
        _bookService = bookService;
        _reviewService = reviewService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll() => Ok(_bookService.GetAll());

    [HttpGet("{id}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = _bookService.GetById(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public ActionResult<Book> Create(BookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            PublishedYear = request.PublishedYear
        };
        var created = _bookService.Add(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public ActionResult<Book> Update(int id, BookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            PublishedYear = request.PublishedYear
        };
        var updated = _bookService.Update(id, book);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_bookService.Delete(id))
        {
            return NotFound();
        }

        _reviewService.DeleteForBook(id);
        return NoContent();
    }
}
