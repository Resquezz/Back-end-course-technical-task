namespace task.DTOs
{
    public class ProposedMeetingDto
    {
        public ProposedMeetingDto(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}
