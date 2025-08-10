using Microsoft.AspNetCore.Mvc;
using task.Data_Storages;
using task.DTOs;
using task.Models;
using task.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly ISlotFinderService _slotFinderService;
        private readonly IMeetingDataStorage _dataStorage;

        public MeetingsController(ISlotFinderService slotFinderService, IMeetingDataStorage dataStorage)
        {
            _slotFinderService = slotFinderService;
            _dataStorage = dataStorage;
        }

        // POST /meetings
        [HttpPost]
        public ActionResult<ProposedMeetingDto> FindEarliestSlot(CreateMeetingRequest requestData)
        {
            if (requestData.DurationMinutes <= 0)
                return BadRequest("Duration must be a positive number.");
            if (requestData.ParticipantIds == null || requestData.ParticipantIds.Count <= 0)
                return BadRequest("Participants are required.");

            var slot = _slotFinderService.FindEarliestSlot(requestData);

            if (slot == null)
                return NotFound("No availiable slot.");

            _dataStorage.AddMeeting(new Meeting(requestData.ParticipantIds, slot.Start, slot.End));

            return Ok(slot);
        }
    }
}
