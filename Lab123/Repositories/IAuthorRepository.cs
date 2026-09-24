using ThuVien_API.Models.Domain;
using ThuVien_API.Models.DTO;
namespace ThuVien_API.Repositories
{
    public interface IAuthorRepository
    {
        List<AuthorDTO> GellAllAuthors();
        AuthorNoIdDTO GetAuthorById(int id);
        AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO);
        AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO);
        Author? DeleteAuthorById(int id);
        List<BookWithAuthorAndPublisherDTO> GetBooksByAuthorId(int id);

    }
}
