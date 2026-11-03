using System.ComponentModel.DataAnnotations;

namespace BookNexusAPI.DTOs.Books
{
    public class AddBookDto
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Author { get; set; } = string.Empty;
        [Required, StringLength(250)]
        public string Image { get; set; } = string.Empty;
        [Required, StringLength(250)]
        public string Description { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Genre { get; set; } = string.Empty;
        [Range(0, int.MinValue)]
        public int Quantity { get; set; }
        [Required, StringLength(50)]
        public string Language { get; set; } = string.Empty;
        [Range(0.01, 1_000_000)]
        public decimal Price { get; set; }
    }
}
