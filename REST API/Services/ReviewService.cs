using System.Collections.Concurrent;
using REST_API.Models;

namespace REST_API.Services;

public class ReviewService : IReviewService
{
    private readonly ConcurrentDictionary<int, Review> _reviews = new();
    private int _nextId = 0;

    public IEnumerable<Review> GetAll() =>
        _reviews.Values.OrderBy(r => r.Id);

    public IEnumerable<Review> GetAllForBook(int bookId) =>
        _reviews.Values.Where(r => r.BookId == bookId);

    public Review? GetById(int bookId, int id) =>
        _reviews.TryGetValue(id, out var review) && review.BookId == bookId ? review : null;

    public Review Add(int bookId, Review review)
    {
        review.Id = Interlocked.Increment(ref _nextId);
        review.BookId = bookId;
        review.CreatedAt = DateTime.UtcNow;
        _reviews[review.Id] = review;
        return review;
    }

    public Review? Update(int bookId, int id, Review updated)
    {
        if (!_reviews.TryGetValue(id, out var existing) || existing.BookId != bookId)
            return null;
        updated.Id = id;
        updated.BookId = bookId;
        updated.CreatedAt = existing.CreatedAt;
        _reviews[id] = updated;
        return updated;
    }

    public bool Delete(int bookId, int id)
    {
        if (_reviews.TryGetValue(id, out var review) && review.BookId == bookId)
            return _reviews.TryRemove(id, out _);
        return false;
    }

    public void DeleteForBook(int bookId)
    {
        var reviewIds = _reviews.Values
            .Where(r => r.BookId == bookId)
            .Select(r => r.Id)
            .ToList();

        foreach (var reviewId in reviewIds)
        {
            _reviews.TryRemove(reviewId, out _);
        }
    }
}
