using ThuVien_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThuVien_API.Models.DTO;
using Microsoft.EntityFrameworkCore;


namespace ThuVien_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public BooksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET http://localhost:port/api/get-all-books
        [HttpGet("get-all-books")]
        public async Task<IActionResult> GetAll()
        {
            // var allBooksDomain = _dbContext.Books.ToList();
            //Get Data From Database -Domain Model
            var allBooksDomain = _dbContext.Books;
            //Map domain models to DTOs
            var allBooksDTO = await allBooksDomain.Select(Books => new BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToListAsync();
            //return DTOs
            return Ok(allBooksDTO);
        }

        [HttpGet]
        [Route("get-book-by-id/{id:int}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            //get bookDomain object from DB
            var bookDomain = await _dbContext.Books.Include(b => b.Publisher).Include(b =>
            b.Book_Authors).ThenInclude(ba => ba.Author).FirstOrDefaultAsync(b => b.Id == id); ;
            if (bookDomain == null)
            {
                return NotFound();
            }
            //map bookDomain object to DTO
            var bookDTO = new Models.DTO.BookWithAuthorAndPublisherDTO()
            {
                Id = bookDomain.Id,
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.DateRead,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                DateAdded = bookDomain.DateAdded,
                PublisherName = bookDomain.Publisher != null ? bookDomain.Publisher.Name : "Unknown",
                AuthorNames = bookDomain.Book_Authors?.Where(y => y.Author != null).Select(y =>
               y.Author.FullName).ToList() ?? new List<string>()
            };
            return Ok(bookDTO);
        }

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            // check if publisher exists or not
            var publisherDomain = await _dbContext.Publishers.FirstOrDefaultAsync(x => x.Id ==
            addBookRequestDTO.PublisherID);
            if (publisherDomain == null)
            {
                return NotFound(new { message = "Không tìm thấy NXB" });
            }
            // create a new book domain object
            var bookDomain = new Models.Domain.Book()
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = publisherDomain.Id
            };
            _dbContext.Books.Add(bookDomain);
            await _dbContext.SaveChangesAsync();
            // check authors if they exist and then add to Book_Author table
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == authorId);
                if (authorDomain == null)
                {
                    return NotFound(new { message = "Không tìm thấy tác giả" });
                }
                var bookAuthorDomain = new Models.Domain.Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };
                _dbContext.Books_Authors.Add(bookAuthorDomain);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPut("update-book-by-id/{id:int}")]
        public async Task<IActionResult> UpdateBookById(int id, [FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomain = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDomain != null)
            {
                bookDomain.Title = addBookRequestDTO.Title;
                bookDomain.Description = addBookRequestDTO.Description;
                bookDomain.IsRead = addBookRequestDTO.IsRead;
                bookDomain.DateRead = addBookRequestDTO.DateRead;
                bookDomain.Rate = addBookRequestDTO.Rate;
                bookDomain.Genre = addBookRequestDTO.Genre;
                bookDomain.CoverUrl = addBookRequestDTO.CoverUrl;
                bookDomain.DateAdded = addBookRequestDTO.DateAdded;
                bookDomain.PublisherID = addBookRequestDTO.PublisherID;
                await _dbContext.SaveChangesAsync();
            }
            var existingBookAuthors = await _dbContext.Books_Authors.Where(x => x.BookId == id).ToListAsync();
            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(existingBookAuthors);
                await _dbContext.SaveChangesAsync();
            }
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = await _dbContext.Authors.FirstOrDefaultAsync(x => x.Id == authorId);
                if (authorDomain == null)
                {
                    return NotFound();
                }
                var bookAuthorDomain = new Models.Domain.Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };
                _dbContext.Books_Authors.Add(bookAuthorDomain);
                await _dbContext.SaveChangesAsync();
            }
            return Ok(addBookRequestDTO);
        }

        [HttpDelete("delete-book-by-id/{id:int}")]
        public async Task<IActionResult> DeleteBookById(int id)
        {
            var bookDomain = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDomain == null)
            {
                return NotFound();
            }
            var existingBookAuthors = await _dbContext.Books_Authors.Where(x => x.BookId == id).ToListAsync();
            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(existingBookAuthors);
                await _dbContext.SaveChangesAsync();
            }
            _dbContext.Books.Remove(bookDomain);
            await _dbContext.SaveChangesAsync();
            return Ok(bookDomain);
        }
    }
}