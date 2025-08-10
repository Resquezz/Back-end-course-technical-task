# Back-end-course-technical-task
Meeting Scheduler

This is a backend application for scheduling meetings by finding the earliest available time slot for a given set of participants, duration, and time constraints. The application is built using ASP.NET Core Web API, with in-memory data storage and unit tests using NUnit and Moq.
Setup Instructions

Prerequisites
.NET SDK: Version 8.0 or higher (download from https://dotnet.microsoft.com/download).
Git: To clone the repository.
A code editor like Visual Studio, Visual Studio Code, or Rider.

Steps to Set Up

Clone the Repository:
git clone https://github.com/Resquezz/Back-end-course-technical-task.git
cd Back-end-course-technical-task

Restore Dependencies: Run the following command to restore the required NuGet packages:
dotnet restore

Build the Project: Compile the project to ensure there are no errors:
dotnet build

Run the Application: Start the application:
dotnet run

Access Swagger UI (optional): If running in development mode, open https://localhost:xxxx/swagger in a browser to explore the API endpoints.

Run Unit Tests: To execute the unit tests included in the task.Tests project:
dotnet test

API Endpoints

POST /api/meetings: Find and schedule the earliest available meeting slot for given participants and time constraints.
Request body: CreateMeetingRequest (ParticipantIds, DurationMinutes, EarliestStart, LatestEnd).
Response: ProposedMeetingDto with the meeting's start and end times or an error if no slot is available.

POST /api/users: Create a new user.
Request body: CreateUserDto (Name).
Response: The created User object.

GET /api/users/{id}/meetings: Retrieve all meetings for a specific user by their ID.

Dependencies
ASP.NET Core 8.0
NUnit (for unit testing)
Moq (for mocking dependencies)
Swagger (for API documentation)

Known Limitations and Edge Cases
In-Memory Storage: The application uses InMemoryMeetingDataStorage, which means all data (users and meetings) is lost when the application restarts. This is suitable for testing but not for production use.
Business Hours Restriction: The SlotFinderService only schedules meetings between 9:00 AM and 5:00 PM on the specified date. Requests for meetings outside these hours (e.g., before 9:00 AM or after 5:00 PM) will return null.
