using task.DTOs;

namespace task.Services
{
    public interface ISlotFinderService
    {
        ProposedMeetingDto? FindEarliestSlot(CreateMeetingRequest requestData);
    }
}
