using Microsoft.AspNetCore.Mvc;
using ThuVien_API.Data;
using ThuVien_API.Models.DTO;
using ThuVien_API.Repositories;

namespace ThuVien_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisherRepository _publisherRepository;

        public PublishersController(AppDbContext dbContext, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-publisher")]
        public IActionResult GetAllPublisher([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var allPublishers = _publisherRepository.GetAllPublishers(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            return Ok(allPublishers);
        }


        [HttpGet("get-publisher-by-id")]
        public IActionResult GetPublisherById(int id)
        {
            var publisherWithId = _publisherRepository.GetPublisherById(id);
            return Ok(publisherWithId);
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            if (_publisherRepository.ExistsPublisherName(addPublisherRequestDTO.Name))
            {
                ModelState.AddModelError(
                    nameof(addPublisherRequestDTO.Name),
                    $"{nameof(addPublisherRequestDTO.Name)} đã tồn tại"
                );
                return BadRequest(ModelState);
            }

            var publisherAdd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
            return Ok(publisherAdd);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherDTO)
        {
            if (_publisherRepository.ExistsPublisherName(publisherDTO.Name, id))
            {
                ModelState.AddModelError(
                    nameof(publisherDTO.Name),
                    $"{nameof(publisherDTO.Name)} đã tồn tại"
                );
                return BadRequest(ModelState);
            }

            var publisherUpdate = _publisherRepository.UpdatePublisherById(id, publisherDTO);

            return Ok(publisherUpdate);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            var publisherDelete = _publisherRepository.DeletePublisherById(id);
            return Ok();
        }

        [HttpGet("{id}/books")]
        public IActionResult GetBooksByPublisherId(int id)
        {
            var booksOfPublisher = _publisherRepository.GetBooksByPublisherId(id);
            return Ok(booksOfPublisher);
        }

    }
}
