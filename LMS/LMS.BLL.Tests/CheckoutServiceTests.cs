using System;
using System.Threading.Tasks;
using FluentAssertions;
using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Repositories.Interfaces;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using LMS.Domain.ViewModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LMS.BLL.Tests.Services
{
    public class CheckoutServiceTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IPaymobService> _paymobServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        
        private readonly CheckoutService _sut; // System Under Test

        public CheckoutServiceTests()
        {
            // Initialize Mocks
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _paymobServiceMock = new Mock<IPaymobService>();
            _configurationMock = new Mock<IConfiguration>();
            _notificationServiceMock = new Mock<INotificationService>();

            // UserManager requires a bit of setup to mock correctly
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Inject mocks into the CheckoutService
            _sut = new CheckoutService(
                _paymentRepositoryMock.Object,
                _courseRepositoryMock.Object,
                _paymobServiceMock.Object,
                _configurationMock.Object,
                _notificationServiceMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task InitiateCheckoutAsync_ValidFreeCourse_ShouldSaveNotificationAndTriggerSignalR()
        {
            // Arrange
            int validCourseId = 1;
            string studentId = "student-123";
            string email = "student@test.com";
            string name = "John Doe";

            var mockCourse = new CourseViewModel 
            { 
                Id = validCourseId, 
                Title = "Test Driven Development in C#", 
                Price = 0 // Free course triggers the direct enrollment flow
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseByIdAsync(validCourseId))
                .ReturnsAsync(mockCourse);

            _paymentRepositoryMock
                .Setup(repo => repo.CreateActiveEnrollmentAsync(studentId, validCourseId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.InitiateCheckoutAsync(validCourseId, studentId, email, name);

            // Assert
            // 1. Verify response is successful and marked as free
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFree.Should().BeTrue();
            result.CourseTitle.Should().Be(mockCourse.Title);

            // 2. Verify the purchase record (enrollment) is added
            _paymentRepositoryMock.Verify(
                repo => repo.CreateActiveEnrollmentAsync(studentId, validCourseId), 
                Times.Once);

            // 3. Verify SendToUserAsync is called exactly once for the Student
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToUserAsync(
                    studentId, 
                    "Course Enrolled", 
                    $"You successfully bought the course {mockCourse.Title}", 
                    NotificationType.CoursePurchase), 
                Times.Once);

            // 4. Verify SendToRoleAsync is called exactly once for the Instructor/Admin
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToRoleAsync(
                    "Instructor", 
                    "New Enrollment", 
                    $"Student {name} purchased the course {mockCourse.Title}", 
                    NotificationType.CoursePurchase), 
                Times.Once);
        }

        [Fact]
        public async Task InitiateCheckoutAsync_CourseDoesNotExist_ShouldReturnErrorAndNotSendNotifications()
        {
            // Arrange
            int invalidCourseId = 999;
            string studentId = "student-123";
            string email = "student@test.com";
            string name = "John Doe";

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseByIdAsync(invalidCourseId))
                .ReturnsAsync((CourseViewModel)null); // Course not found

            // Act
            var result = await _sut.InitiateCheckoutAsync(invalidCourseId, studentId, email, name);

            // Assert
            // 1. Verify response indicates failure
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Course not found.");

            // 2. Verify no enrollment or payment was created
            _paymentRepositoryMock.Verify(
                repo => repo.CreateActiveEnrollmentAsync(It.IsAny<string>(), It.IsAny<int>()), 
                Times.Never);
                
            _paymentRepositoryMock.Verify(
                repo => repo.AddPendingPaymentAsync(It.IsAny<Payment>()), 
                Times.Never);

            // 3. Verify no notifications were sent to the user
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToUserAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<NotificationType>()), 
                Times.Never);

            // 4. Verify no notifications were sent to any role
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToRoleAsync(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<NotificationType>()), 
                Times.Never);
        }

        [Fact]
        public async Task InitiateCheckoutAsync_PaidCourse_ShouldCreatePendingPaymentAndReturnPaymobUrl()
        {
            // Arrange
            int validCourseId = 2;
            string studentId = "student-123";
            string email = "student@test.com";
            string name = "John Doe";
            decimal coursePrice = 150.00m;
            string mockedToken = "mock_token_123456";
            string mockedIframeId = "123456";

            var mockCourse = new CourseViewModel 
            { 
                Id = validCourseId, 
                Title = "Advanced ASP.NET Core", 
                Price = coursePrice 
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseByIdAsync(validCourseId))
                .ReturnsAsync(mockCourse);

            _paymentRepositoryMock
                .Setup(repo => repo.AddPendingPaymentAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => 
                {
                    p.Id = 100; // Mock database saving assigning an ID
                    return p;
                });

            _paymobServiceMock
                .Setup(service => service.GetPaymentKeyAsync(coursePrice, email, name, "Student", It.IsAny<string>()))
                .ReturnsAsync(mockedToken);

            _configurationMock
                .Setup(config => config["PAYMOB_IFRAME_ID"])
                .Returns(mockedIframeId);

            // Act
            var result = await _sut.InitiateCheckoutAsync(validCourseId, studentId, email, name);

            // Assert
            // 1. Verify response properties
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.IsFree.Should().BeFalse();
            result.CourseTitle.Should().Be(mockCourse.Title);
            result.CourseId.Should().Be(validCourseId);
            result.PaymobRedirectUrl.Should().NotBeNullOrEmpty();
            result.PaymobRedirectUrl.Should().Contain(mockedIframeId);
            result.PaymobRedirectUrl.Should().Contain(mockedToken);

            // 2. Verify AddPendingPaymentAsync was called exactly once with correct values
            _paymentRepositoryMock.Verify(
                repo => repo.AddPendingPaymentAsync(It.Is<Payment>(p => 
                    p.CourseId == validCourseId && 
                    p.StudentId == studentId && 
                    p.Amount == coursePrice &&
                    p.Status == PaymentStatus.Pending)), 
                Times.Once);

            // 3. Verify GetPaymentKeyAsync was called exactly once
            _paymobServiceMock.Verify(
                service => service.GetPaymentKeyAsync(coursePrice, email, name, "Student", It.IsAny<string>()), 
                Times.Once);

            // 4. Crucial: Ensure NO enrollment is created yet
            _paymentRepositoryMock.Verify(
                repo => repo.CreateActiveEnrollmentAsync(It.IsAny<string>(), It.IsAny<int>()), 
                Times.Never);
                
            // 5. Ensure NO notifications are sent (they should be sent on webhook success)
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()), 
                Times.Never);
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()), 
                Times.Never);
        }

        [Fact]
        public async Task InitiateCheckoutAsync_PaymobServiceFails_ShouldHandleExceptionGracefully()
        {
            // Arrange
            int validCourseId = 3;
            string studentId = "student-123";
            string email = "student@test.com";
            string name = "John Doe";
            decimal coursePrice = 150.00m;

            var mockCourse = new CourseViewModel 
            { 
                Id = validCourseId, 
                Title = "Advanced ASP.NET Core", 
                Price = coursePrice 
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseByIdAsync(validCourseId))
                .ReturnsAsync(mockCourse);

            _paymentRepositoryMock
                .Setup(repo => repo.AddPendingPaymentAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => 
                {
                    p.Id = 101; 
                    return p;
                });

            // Simulate a network failure or API error from Paymob
            _paymobServiceMock
                .Setup(service => service.GetPaymentKeyAsync(coursePrice, email, name, "Student", It.IsAny<string>()))
                .ThrowsAsync(new Exception("Paymob API is currently down."));

            // Act
            Func<Task> act = async () => await _sut.InitiateCheckoutAsync(validCourseId, studentId, email, name);

            // Assert
            // 1. Verify that the exception correctly bubbles up (as expected in this architecture)
            await act.Should().ThrowAsync<Exception>().WithMessage("Paymob API is currently down.");

            // 2. Verify no active enrollment was accidentally created
            _paymentRepositoryMock.Verify(
                repo => repo.CreateActiveEnrollmentAsync(It.IsAny<string>(), It.IsAny<int>()), 
                Times.Never);
                
            // 3. Verify no notifications were mistakenly fired
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()), 
                Times.Never);
            _notificationServiceMock.Verify(
                service => service.CreateAndSendToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()), 
                Times.Never);
        }
    }
}
