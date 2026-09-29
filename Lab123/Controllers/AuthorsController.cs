using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuVien_API.Data;
using ThuVien_API.Models.DTO;
using ThuVien_API.Repositories;

namespace ThuVien_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorRepository = authorRepository;
        }

        [Authorize(Roles = "Read")]
        [HttpGet("get-all-author")]
        public IActionResult GetAllAuthor([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
             [FromQuery] string? sortBy, [FromQuery] bool isAscending,
             [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var allAuthors = _authorRepository.GellAllAuthors(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(allAuthors);
        }


        [Authorize(Roles = "Read")]
        [HttpGet("get-author-by-id/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var authorWithId = _authorRepository.GetAuthorById(id);
            return Ok(authorWithId);
        }

        [Authorize(Roles = "Write")]
        [HttpPost("add-author")]
        public IActionResult AddAuthors([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
            return Ok();
        }

        [Authorize(Roles = "Write")]
        [HttpPut("update-author-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AuthorNoIdDTO authorDTO)
        {
            var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
            return Ok(authorUpdate);
        }

        [Authorize(Roles = "Write")]
        [HttpDelete("delete-author-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var authorDelete = _authorRepository.DeleteAuthorById(id);
            return Ok();
        }

        [Authorize(Roles = "Read")]
        [HttpGet("{id}/books")]
        public IActionResult GetBooksByAuthorId(int id)
        {
            var booksOfAuthor = _authorRepository.GetBooksByAuthorId(id);
            return Ok(booksOfAuthor);
        }

    }
}
