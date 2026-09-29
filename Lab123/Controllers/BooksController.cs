using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ThuVien_API.Data;
using ThuVien_API.Models.Domain;
using ThuVien_API.Models.DTO;
using ThuVien_API.Repositories;
using Microsoft.AspNetCore.Authorization;


namespace ThuVien_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(AppDbContext dbContext, IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Read")]
        [HttpGet("get-all-books")]
        public IActionResult GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
             [FromQuery] string? sortBy, [FromQuery] bool isAscending,
             [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");

            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is a error log");

            // su dung reposity pattern
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            //debug
            _logger.LogInformation($"Finished GetAllBook request with data {System.Text.Json.JsonSerializer.Serialize(allBooks)}");
            return Ok(allBooks);
        }



        [Authorize(Roles = "Read")]
        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }


        [Authorize(Roles = "Write")]
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (!ValidateAddBook(addBookRequestDTO))
            {
                return BadRequest(ModelState);
            }

            var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);        
        }


        [Authorize(Roles = "Write")]
        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            if (!_bookRepository.ExistsPublisherId(bookDTO.PublisherID))
            {
                ModelState.AddModelError(
                    nameof(bookDTO.PublisherID),
                    $"{nameof(bookDTO.PublisherID)} không tồn tại"
                );
                return BadRequest(ModelState);
            }

            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            return Ok(updateBook);
        }


        [Authorize(Roles = "Write")]
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }

        #region Private methods

        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO),
                    $"Please add book data"
                );

                return false;
            }

            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Description),
                    $"{nameof(addBookRequestDTO.Description)} cannot be null"
                );
            }

            if (addBookRequestDTO.Rate < 0 ||
                addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.Rate),
                    $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5"
                );
            }
            if (!_bookRepository.ExistsPublisherId(addBookRequestDTO.PublisherID))
            {
                ModelState.AddModelError(
                    nameof(addBookRequestDTO.PublisherID),
                    $"{nameof(addBookRequestDTO.PublisherID)} không tồn tại"
                );
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
