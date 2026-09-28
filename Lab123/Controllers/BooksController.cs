using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ThuVien_API.Data;
using ThuVien_API.Models.Domain;
using ThuVien_API.Models.DTO;
using ThuVien_API.Repositories;

namespace ThuVien_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;

        public BooksController(AppDbContext dbContext, IBookRepository bookRepository)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            // su dung reposity pattern
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery);
            return Ok(allBooks);
        }


        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }
      
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
