using System.Collections.Concurrent;
using REST_API.Models;

namespace REST_API.Services;

public class BookService : IBookService
{
    private readonly ConcurrentDictionary<int, Book> _books = new();
    private int _nextId = 0;

    public IEnumerable<Book> GetAll() => _books.Values;

    public Book? GetById(int id) => _books.TryGetValue(id, out var book) ? book : null;

    public Book Add(Book book)
    {
        book.Id = Interlocked.Increment(ref _nextId);
        _books[book.Id] = book;
        return book;
    }

    public Book? Update(int id, Book updated)
    {
        if (!_books.ContainsKey(id)) return null;
        updated.Id = id;
        _books[id] = updated;
        return updated;
    }

    public bool Delete(int id) => _books.TryRemove(id, out _);
}
