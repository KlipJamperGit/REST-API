using REST_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddSingleton<IReviewService, ReviewService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// Serve the main HTML page
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="uk">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Book Review API</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f4f6f8;
            color: #1f2933;
            margin: 0;
            min-height: 100vh;
            padding: 2rem;
        }

        .container {
            background: white;
            padding: 2rem;
            border-radius: 12px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.08);
            max-width: 900px;
            width: min(100%, 900px);
            margin: 0 auto;
        }

        h1 {
            margin-top: 0;
            color: #0f172a;
        }

        p {
            line-height: 1.6;
        }

        .actions {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            margin: 1.5rem 0;
        }

        button {
            border: none;
            border-radius: 8px;
            padding: 0.85rem 1.2rem;
            font-size: 1rem;
            cursor: pointer;
            background: #2563eb;
            color: white;
        }

        button:hover {
            background: #1d4ed8;
        }

        .delete-button {
            background: #dc2626;
        }

        .delete-button:hover {
            background: #b91c1c;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
            gap: 1.5rem;
            margin-top: 1.5rem;
        }

        .card {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 1rem;
        }

        form {
            display: grid;
            gap: 0.75rem;
        }

        input,
        select {
            padding: 0.75rem;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            font-size: 1rem;
        }

        .status {
            margin-top: 0.75rem;
            font-weight: 600;
        }

        .action-status {
            margin-top: -0.5rem;
            margin-bottom: 1rem;
            color: #334155;
            font-weight: 600;
        }

        .list {
            margin: 0;
            padding-left: 1.2rem;
        }

        .list li {
            margin-bottom: 0.6rem;
        }

        .book-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 0.75rem;
        }

        .book-info {
            cursor: pointer;
            flex: 1;
        }

        .book-info:hover {
            color: #2563eb;
        }

        .rating {
            color: #b45309;
            font-weight: 600;
        }

        .book-actions {
            display: flex;
            gap: 0.5rem;
        }

        .book-actions button {
            padding: 0.45rem 0.8rem;
            font-size: 0.9rem;
        }

        .empty {
            color: #64748b;
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>Book Review REST API</h1>
        <p>Сервіс успішно запущено.</p>
        <p>Керуйте книгами та переглядайте відгуки прямо з головної сторінки.</p>

        <div class="actions">
            <button type="button" onclick="loadBooks()">Завантажені книги</button>
            <button type="button" onclick="loadReviews()">Завантажені відгуки</button>
        </div>
        <div id="actionStatus" class="action-status"></div>

        <div class="grid">
            <section class="card">
                <h2>Додати книгу</h2>
                <form id="bookForm">
                    <input id="title" name="title" placeholder="Назва книги" required />
                    <input id="author" name="author" placeholder="Автор" required />
                    <input id="genre" name="genre" placeholder="Жанр" />
                    <input id="publishedYear" name="publishedYear" type="number" placeholder="Рік публікації" min="1" max="2100" required />
                    <button type="submit">Зберегти книгу</button>
                </form>
                <div id="bookStatus" class="status"></div>
            </section>

            <section class="card">
                <h2>Додати відгук</h2>
                <form id="reviewForm">
                    <select id="reviewBookId" name="reviewBookId" required>
                        <option value="">Оберіть книгу</option>
                    </select>
                    <input id="reviewerName" name="reviewerName" placeholder="Ім'я автора відгуку" required />
                    <input id="rating" name="rating" type="number" placeholder="Оцінка від 1 до 5" min="1" max="5" required />
                    <input id="comment" name="comment" placeholder="Коментар" />
                    <button type="submit">Зберегти відгук</button>
                </form>
                <div id="reviewStatus" class="status"></div>
            </section>

            <section class="card">
                <h2>Список книг</h2>
                <ul id="booksList" class="list"></ul>
            </section>

            <section class="card">
                <h2>Список відгуків</h2>
                <ul id="reviewsList" class="list"></ul>
            </section>
        </div>
    </div>

    <script>
        const booksList = document.getElementById('booksList');
        const reviewsList = document.getElementById('reviewsList');
        const bookForm = document.getElementById('bookForm');
        const reviewForm = document.getElementById('reviewForm');
        const reviewBookId = document.getElementById('reviewBookId');
        const bookStatus = document.getElementById('bookStatus');
        const reviewStatus = document.getElementById('reviewStatus');
        const actionStatus = document.getElementById('actionStatus');
        let currentBooks = [];
        let currentReviews = [];

        function renderEmpty(target, message) {
            target.innerHTML = `<li class="empty">${message}</li>`;
        }

        function updateBookOptions(books) {
            if (!books.length) {
                reviewBookId.innerHTML = '<option value="">Спочатку додайте книгу</option>';
                reviewBookId.disabled = true;
                return;
            }

            reviewBookId.disabled = false;
            reviewBookId.innerHTML = `
                <option value="">Оберіть книгу</option>
                ${books.map(book => `<option value="${book.id}">#${book.id} ${book.title}</option>`).join('')}
            `;
        }

        function getAverageRating(bookId) {
            const bookReviews = currentReviews.filter(review => review.bookId === bookId);

            if (!bookReviews.length) {
                return 'Немає оцінок';
            }

            const average = bookReviews.reduce((sum, review) => sum + review.rating, 0) / bookReviews.length;
            return `${average.toFixed(1)}/5`;
        }

        function renderBooks(books) {
            if (!books.length) {
                renderEmpty(booksList, 'Книги ще не додані.');
                return;
            }

            booksList.innerHTML = books
                .map(book => `
                    <li class="book-item">
                        <span class="book-info" onclick="showBookReviews(${book.id})">
                            <strong>#${book.id} ${book.title}</strong> — ${book.author} (${book.publishedYear})<br>
                            <span class="rating">Рейтинг: ${getAverageRating(book.id)}</span>
                        </span>
                        <span class="book-actions">
                            <button type="button" class="delete-button" onclick="deleteBook(${book.id})">Видалити</button>
                        </span>
                    </li>`)
                .join('');
        }

        function renderReviews(reviews, selectedBookId = null) {
            if (!reviews.length) {
                renderEmpty(reviewsList, selectedBookId ? 'Для цієї книги відгуків ще немає.' : 'Відгуків ще немає.');
                return;
            }

            const booksById = Object.fromEntries(currentBooks.map(book => [book.id, book]));

            reviewsList.innerHTML = reviews
                .map(review => {
                    const book = booksById[review.bookId];
                    const bookTitle = book ? book.title : `Книга #${review.bookId}`;
                    return `<li><strong>${review.reviewerName}</strong> — ${bookTitle} — оцінка ${review.rating}/5<br>${review.comment}</li>`;
                })
                .join('');
        }

        async function showBookReviews(bookId) {
            const selectedBook = currentBooks.find(book => book.id === bookId);
            const filteredReviews = currentReviews.filter(review => review.bookId === bookId);

            renderReviews(filteredReviews, bookId);
            actionStatus.textContent = selectedBook
                ? `Показано відгуки для книги: ${selectedBook.title}`
                : `Показано відгуки для книги #${bookId}`;
        }

        async function loadBooks() {
            actionStatus.textContent = 'Завантаження книг...';

            try {
                const response = await fetch('/api/books');

                if (!response.ok) {
                    throw new Error('Помилка завантаження книг.');
                }

                const books = await response.json();
                currentBooks = books;
                updateBookOptions(books);
                renderBooks(books);

                actionStatus.textContent = books.length
                    ? `Завантажено книг: ${books.length}. Натисни на книгу, щоб побачити її відгуки.`
                    : 'Завантажено 0 книг.';
            } catch {
                currentBooks = [];
                updateBookOptions([]);
                renderEmpty(booksList, 'Не вдалося завантажити книги.');
                actionStatus.textContent = 'Помилка при завантаженні книг.';
            }
        }

        async function loadReviews() {
            actionStatus.textContent = 'Завантаження відгуків...';

            try {
                const response = await fetch('/api/reviews');

                if (!response.ok) {
                    throw new Error('Помилка завантаження відгуків.');
                }

                const reviews = await response.json();
                currentReviews = reviews;
                renderReviews(reviews);

                renderBooks(currentBooks);

                actionStatus.textContent = `Завантажено відгуків: ${reviews.length}.`;
            } catch {
                currentReviews = [];
                renderEmpty(reviewsList, 'Не вдалося завантажити відгуки.');
                actionStatus.textContent = 'Помилка при завантаженні відгуків.';
            }
        }

        async function deleteBook(bookId) {
            actionStatus.textContent = `Видалення книги #${bookId}...`;

            try {
                const response = await fetch(`/api/books/${bookId}`, {
                    method: 'DELETE'
                });

                if (!response.ok) {
                    throw new Error('Помилка видалення книги.');
                }

                actionStatus.textContent = `Книгу #${bookId} видалено.`;
                await loadBooks();
                await loadReviews();
            } catch {
                actionStatus.textContent = `Не вдалося видалити книгу #${bookId}.`;
            }
        }

        bookForm.addEventListener('submit', async (event) => {
            event.preventDefault();

            const payload = {
                title: document.getElementById('title').value,
                author: document.getElementById('author').value,
                genre: document.getElementById('genre').value,
                publishedYear: Number(document.getElementById('publishedYear').value)
            };

            const response = await fetch('/api/books', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                bookStatus.textContent = 'Не вдалося зберегти книгу.';
                return;
            }

            bookStatus.textContent = 'Книгу успішно збережено.';
            actionStatus.textContent = 'Список книг оновлено.';
            bookForm.reset();
            await loadBooks();
            await loadReviews();
        });

        reviewForm.addEventListener('submit', async (event) => {
            event.preventDefault();

            const bookId = Number(reviewBookId.value);
            const payload = {
                reviewerName: document.getElementById('reviewerName').value,
                rating: Number(document.getElementById('rating').value),
                comment: document.getElementById('comment').value
            };

            const response = await fetch(`/api/books/${bookId}/reviews`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                reviewStatus.textContent = 'Не вдалося зберегти відгук. Перевір ID книги.';
                return;
            }

            reviewStatus.textContent = 'Відгук успішно збережено.';
            reviewForm.reset();
            reviewBookId.value = '';
            await loadReviews();
            renderBooks(currentBooks);
        });

        renderEmpty(booksList, 'Завантаження книг...');
        renderEmpty(reviewsList, 'Натисни «Завантажити відгуки».');
        loadBooks();
        loadReviews();
    </script>
</body>
</html>
""", "text/html"));

app.Run();
