using BookNexusAPI.DTOs;

namespace BookNexusAPI.Services
{
    public interface IBookService
    {
        public Task<List<BookDto>> GetAllAsync();
        public Task<BookDto?> GetByIdAsync(int id);
        public Task<BookDto> AddAsync(AddBookDto dto);
    }
}
