using task.Data_Storages;
using task.DTOs;
using task.Models;

namespace task.Services
{
    public class SlotFinderService : ISlotFinderService
    {
        private readonly IMeetingDataStorage _dataStorage;
        private const int _businessDayStartHour = 9;
        private const int _businessDayEndHour = 17;
        public SlotFinderService(IMeetingDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public ProposedMeetingDto? FindEarliestSlot(CreateMeetingRequest requestData)
        {
            DateTime date = requestData.EarliestStart.Date;
            DateTimeOffset businessDayStart = new DateTimeOffset(date.Year, date.Month, date.Day,
                _businessDayStartHour, 0, 0, TimeSpan.Zero);
            DateTimeOffset businessDayEnd = new DateTimeOffset(date.Year, date.Month, date.Day,
                _businessDayEndHour, 0, 0, TimeSpan.Zero);
            if (requestData.LatestEnd <= businessDayStart || requestData.EarliestStart >= businessDayEnd)
                return null;

            double duration = requestData.DurationMinutes;
            DateTimeOffset earliestTimeAllowed = requestData.EarliestStart < businessDayStart
                ? businessDayStart
                : requestData.EarliestStart;
            DateTimeOffset latestTimeAllowed = requestData.LatestEnd > businessDayEnd
                ? businessDayEnd
                : requestData.LatestEnd;

            var meetings = _dataStorage.GetAllMeetings()
                .Where(meet => meet.ParticipantIds.Any(id => requestData.ParticipantIds.Contains(id)))
                .Where(meet => meet.EndTime > earliestTimeAllowed && meet.StartTime < latestTimeAllowed)
                .ToList();

            var sortedMeetings = meetings.OrderBy(meeting => meeting.StartTime).ToList();

            return FindSlot(sortedMeetings, earliestTimeAllowed, latestTimeAllowed, duration);
        }

        private ProposedMeetingDto? FindSlot(List<Meeting> sortedMeetings, DateTimeOffset earliestTimeAllowed,
            DateTimeOffset latestTimeAllowed, double duration)
        {
            if (!sortedMeetings.Any() || (sortedMeetings[0].StartTime - earliestTimeAllowed).TotalMinutes >= duration)
            {
                return CreateProposedMeeting(earliestTimeAllowed, duration);
            }
            for (int i = 0; i < sortedMeetings.Count - 1; ++i)
            {
                if ((sortedMeetings[i + 1].StartTime - sortedMeetings[i].EndTime).TotalMinutes >= duration)
                {
                    return CreateProposedMeeting(sortedMeetings[i].EndTime, duration);
                }
            }
            if ((latestTimeAllowed - sortedMeetings[sortedMeetings.Count - 1].EndTime).TotalMinutes >= duration)
            {
                return CreateProposedMeeting(sortedMeetings[sortedMeetings.Count - 1].EndTime, duration);
            }
            return null;
        }

        private ProposedMeetingDto CreateProposedMeeting(DateTimeOffset startTime, double duration)
        {
            return new ProposedMeetingDto(startTime, startTime.AddMinutes(duration));
        }
    }
}
