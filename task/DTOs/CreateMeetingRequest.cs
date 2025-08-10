namespace task.DTOs
{
    public class CreateMeetingRequest
    {
        public List<int> ParticipantIds { get; set; }
        public double DurationMinutes { get; set; }
        public DateTimeOffset EarliestStart { get; set; }
        public DateTimeOffset LatestEnd { get; set; }
    }
}
