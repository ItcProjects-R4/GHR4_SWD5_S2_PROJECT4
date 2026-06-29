using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace LMS.BLL.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IRealTimeNotificationService> _realTimeServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly NotificationService _sut;

        public NotificationServiceTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _realTimeServiceMock = new Mock<IRealTimeNotificationService>();
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            _sut = new NotificationService(_contextMock.Object, _realTimeServiceMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task MarkAllAsReadAsync_HasUnreadNotifications_ShouldMarkAsReadAndSave()
        {
            // Arrange
            string userId = "user1";
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, IsRead = false },
                new Notification { Id = 2, UserId = userId, IsRead = false },
                new Notification { Id = 3, UserId = "user2", IsRead = false },
                new Notification { Id = 4, UserId = userId, IsRead = true }
            };

            // Using Moq.EntityFrameworkCore to mock DbSet natively
            _contextMock.Setup(c => c.Notifications).ReturnsDbSet(notifications);

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // Return 1 to indicate save success

            // Act
            await _sut.MarkAllAsReadAsync(userId);

            // Assert
            // 1. Check if the specific unread notifications belonging to the user are now marked as read
            notifications.First(n => n.Id == 1).IsRead.Should().BeTrue();
            notifications.First(n => n.Id == 2).IsRead.Should().BeTrue();

            // 2. Ensure other users' notifications were not touched
            notifications.First(n => n.Id == 3).IsRead.Should().BeFalse();

            // 3. Ensure already read notifications weren't "re-read" (optional, but conceptually sound)
            notifications.First(n => n.Id == 4).IsRead.Should().BeTrue();

            // 4. Verify SaveChangesAsync was called exactly once to commit the updates
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
