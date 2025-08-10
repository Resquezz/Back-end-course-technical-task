using task.Models;

namespace task.Data_Storages
{
    public interface IMeetingDataStorage
    {
        User AddUser(User user);
        User? GetUser(int id);
        List<User> GetAllUsers();
        Meeting AddMeeting(Meeting meeting);
        List<Meeting> GetMeetingsForUser(int userId);
        List<Meeting> GetAllMeetings();
    }
}
