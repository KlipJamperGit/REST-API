using REST_API.Models;

namespace REST_API.Services;

public interface IReviewService
{
    IEnumerable<Review> GetAll();
    IEnumerable<Review> GetAllForBook(int bookId);
    Review? GetById(int bookId, int id);
    Review Add(int bookId, Review review);
    Review? Update(int bookId, int id, Review review);
    bool Delete(int bookId, int id);
    void DeleteForBook(int bookId);
}
