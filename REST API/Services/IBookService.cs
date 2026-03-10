using REST_API.Models;

namespace REST_API.Services;

public interface IBookService
{
    IEnumerable<Book> GetAll();
    Book? GetById(int id);
    Book Add(Book book);
    Book? Update(int id, Book book);
    bool Delete(int id);
}
