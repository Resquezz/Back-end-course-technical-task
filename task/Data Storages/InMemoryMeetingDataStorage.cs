using task.Models;

namespace task.Data_Storages
{
    public class InMemoryMeetingDataStorage : IMeetingDataStorage
    {
        private List<User> _users = new List<User>();
        private List<Meeting> _meetings = new List<Meeting>();
        private int _userIdCounter = 1;
        private int _meetingIdCounter = 1;
        public Meeting AddMeeting(Meeting meeting)
        {
            meeting.Id = _meetingIdCounter++;
            _meetings.Add(meeting);
            return meeting;
        }

        public User AddUser(User user)
        {
            user.Id = _userIdCounter++;
            _users.Add(user);
            return user;
        }

        public List<Meeting> GetAllMeetings() => _meetings.ToList();

        public List<User> GetAllUsers() => _users.ToList();

        public List<Meeting> GetMeetingsForUser(int userId)
        {
            return _meetings
                .Where(meeting => meeting.ParticipantIds
                .Contains(userId))
                .ToList();
        }

        public User? GetUser(int id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }
    }
}
