namespace task.Models
{
    public class Meeting
    {
        public Meeting(List<int> participantIds, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            ParticipantIds = participantIds;
            StartTime = startTime;
            EndTime = endTime;
        }
        public Meeting() { }

        public int Id { get; set; }
        public List<int> ParticipantIds { get; set; } = new List<int>();
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
