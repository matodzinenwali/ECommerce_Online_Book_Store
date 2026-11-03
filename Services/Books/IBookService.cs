using BookNexusAPI.DTOs.Books;

namespace BookNexusAPI.Services.Books
{
    public interface IBookService
    {
        public Task<List<BookDto>> GetAllAsync();
        public Task<BookDto?> GetByIdAsync(int id);
        public Task<BookDto> AddAsync(AddBookDto dto);
    }
}
