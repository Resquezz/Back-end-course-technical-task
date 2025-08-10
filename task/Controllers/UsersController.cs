using Microsoft.AspNetCore.Mvc;
using task.Data_Storages;
using task.DTOs;
using task.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMeetingDataStorage _dataStorage;

        public UsersController(IMeetingDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        // GET: /users/id/meetings
        [HttpGet("{id}/meetings")]
        public ActionResult<List<Meeting>> GetAllUserMeetings(int id)
        {
            if (_dataStorage.GetUser(id) == null)
                return NotFound($"User with id {id} not found.");

            return Ok(_dataStorage.GetMeetingsForUser(id));
        }
        // POST /users
        [HttpPost]
        public ActionResult<User> CreateUser(CreateUserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("Name is required.");

            var user = new User { Name = dto.Name };
            var userInDataStore = _dataStorage.AddUser(user);
            return CreatedAtAction(nameof(GetAllUserMeetings), new { id = userInDataStore.Id }, userInDataStore);
        }
    }
}
