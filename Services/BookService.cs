using BookNexusAPI.Data;
using BookNexusAPI.DTOs;
using BookNexusAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNexusAPI.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _db;

        public BookService(AppDbContext db) => _db = db;

        /*
         * Be aware of duplicate-title race conditions
         * The service checks whether a title exists before inserting, 
         * which is good for normal requests. However, two simultaneous requests 
         * could both pass the check. The unique database index is still necessary, 
         * and eventually you may want to catch DbUpdateException and return 409 Conflict.
         */
        public async Task<BookDto> AddAsync(AddBookDto dto)
        {
            // BUSINESS RULE: titles names must be unique (case insensetive)
            // This rule lives here, not in the controller
            var exists = await _db.Books.AnyAsync(b => b.Title.ToLower() == dto.Title.ToLower());
            if (exists)
                throw new InvalidOperationException($"A book with title '{dto.Title}' already exists");

            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Image = dto.Image,
                Description = dto.Description,
                Genre = dto.Genre,
                Quantity = dto.Quantity,
                Language = dto.Language,
                IsActive = true, // when book is added it becomes active in the database/system
                Price = dto.Price
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            return new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Image = book.Image,
                Description = book.Description,
                Genre = book.Genre,
                Quantity = book.Quantity,
                Language = book.Language,
                IsActive = book.IsActive,
                Price = book.Price
            };
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            return await _db.Books
                    .AsNoTracking()
                    .Select(b => new BookDto
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Author = b.Author,
                        Image = b.Image,
                        Description = b.Description,
                        Genre = b.Genre,
                        Quantity = b.Quantity,
                        Language = b.Language,
                        IsActive = b.IsActive,
                        Price = b.Price
                    })
                    .ToListAsync();
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            return await _db.Books
                .AsNoTracking()
                .Where(b => b.BookId == id)
                .Select(b => new BookDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Image = b.Image,
                    Description = b.Description,
                    Genre = b.Genre,
                    Quantity = b.Quantity,
                    Language = b.Language,
                    IsActive = b.IsActive,
                    Price = b.Price
                })
                .FirstOrDefaultAsync();
        }
    }
}
