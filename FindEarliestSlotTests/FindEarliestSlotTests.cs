using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using task.Data_Storages;
using task.DTOs;
using task.Models;
using task.Services;

namespace task.Tests
{
    [TestFixture]
    public class SlotFinderServiceTests
    {
        private readonly DateTimeOffset _date = new DateTimeOffset(2025, 6, 20, 0, 0, 0, TimeSpan.Zero);
        private CreateMeetingRequest CreateDefaultRequest(List<int> participantsIds, int durationMinutes = 60,
            double earliestStartHour = 9, double latestEndHour = 17)
        {
            return new CreateMeetingRequest
            {
                ParticipantIds = participantsIds,
                DurationMinutes = durationMinutes,
                EarliestStart = _date.AddHours(earliestStartHour),
                LatestEnd = _date.AddHours(latestEndHour)
            };
        }
        private Meeting CreateMeeting(List<int> participantsIds, double startHour, double endHour)
        {
            return new Meeting
            {
                ParticipantIds = participantsIds,
                StartTime = _date.AddHours(startHour),
                EndTime = _date.AddHours(endHour)
            };
        }
        private SlotFinderService CreateService(List<Meeting> meetings)
        {
            var mockStorage = new Mock<IMeetingDataStorage>();
            mockStorage.Setup(s => s.GetAllMeetings()).Returns(meetings);
            return new SlotFinderService(mockStorage.Object);
        }

        [Test]
        public void FindEarliestSlot_ReturnsBusinessDayStart_IfNoMeetings()
        {
            var service = CreateService(new List<Meeting>());
            var request = CreateDefaultRequest(new List<int> { 1, 2 });

            var result = service.FindEarliestSlot(request);

            Assert.That(request.EarliestStart, Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsBusinessDayStart_IfGapBeforeFirstMeeting()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 11, 12)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 });

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(9), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsSlotBetweenMeetings_IfGapExists()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 9, 10),
                CreateMeeting(new List<int> { 1 }, 12, 13)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 });

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(10), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsSlotAfterSecondMeeting_IfFirstGapTooSmall()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 9, 10),
                CreateMeeting(new List<int> { 1 }, 10.5, 12)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 });

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(12), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsSlotAfterLastMeeting_IfNoGaps()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 9, 16)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 });

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(16), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsNull_IfNoSlotAvailable()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 9, 10),
                CreateMeeting(new List<int> { 1 }, 10, 11)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 }, 60, 9, 11);

            var result = service.FindEarliestSlot(request);

            Assert.That(result, Is.Null);
        }
        [Test]
        public void FindEarliestSlot_ReturnsBusinessDayStart_IfNoParticipantOverlap()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 99 }, 9, 10)
            };
            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1 });

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(9), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsBusinessDayStart_IfOutsideBusinessHours()
        {
            var service = CreateService(new List<Meeting>());
            var request = CreateDefaultRequest(new List<int> { 1 }, 60, 7, 20);

            var result = service.FindEarliestSlot(request);

            Assert.That(_date.AddHours(9), Is.EqualTo(result.Start));
        }
        [Test]
        public void FindEarliestSlot_ReturnsNull_IfBeforeBusinessHours()
        {
            var service = CreateService(new List<Meeting>());
            var request = CreateDefaultRequest(new List<int> { 1 }, 30, 7, 8);

            var result = service.FindEarliestSlot(request);

            Assert.That(result, Is.Null);
        }
        [Test]
        public void FindEarliestSlot_ReturnsNull_IfAllPartisipantsBusy()
        {
            var meetings = new List<Meeting>
            {
                CreateMeeting(new List<int> { 1 }, 9, 10),
                CreateMeeting(new List<int> { 2 }, 10, 11),
                CreateMeeting(new List<int> { 3 }, 11, 12),
            };

            var service = CreateService(meetings);
            var request = CreateDefaultRequest(new List<int> { 1, 2, 3 }, 60, 9, 12);

            var result = service.FindEarliestSlot(request);

            Assert.That(result, Is.Null);
        }
    }
}