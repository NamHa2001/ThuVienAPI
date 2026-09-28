using ThuVien_API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace ThuVien_API.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required]
        [MinLength(10)]
        [RegularExpression(@"^[\p{L}\p{N} ]+$", ErrorMessage = "Title chỉ được chứa chữ, số và khoảng trắng")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        //navigation Properties -
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}