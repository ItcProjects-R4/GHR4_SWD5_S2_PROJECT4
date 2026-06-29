using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.BLL.ViewModels;
using LMS.DAL.Data;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using LMS.Domain.Models.LMS.Domain.Models;
using LMS.Domain.ViewModels.Instructor.CourseDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace LMS.BLL.Tests.Services
{
    public class InstructorServiceTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly InstructorService _sut;

        public InstructorServiceTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _notificationServiceMock = new Mock<INotificationService>();

            _sut = new InstructorService(
                _contextMock.Object,
                _currentUserServiceMock.Object,
                _cloudinaryServiceMock.Object,
                _emailSenderMock.Object,
                _notificationServiceMock.Object
            );
        }

        [Fact]
        public async Task GradeSubmissionAsync_ValidSubmission_ShouldUpdateGradeAndNotifyStudent()
        {
            // Arrange
            string instructorId = "instructor1";
            string studentId = "student1";
            int submissionId = 5;
            
            var mockCourse = new Course { Id = 100, Title = "C# Mastery", InstructorId = instructorId, ThumbnailUrl = "", Description = "", Price = 0, Instructor = new ApplicationUser { FirstName = "", LastName = "" } };
            var mockModule = new Module { Id = 10, Course = mockCourse, Title = "Module 1" };
            var mockAssignment = new Assignment { Id = 20, Title = "Midterm", Module = mockModule };
            
            var mockSubmission = new Submission 
            { 
                Id = submissionId, 
                StudentId = studentId, 
                AssignmentId = 20, 
                Assignment = mockAssignment,
                Status = SubmissionStatus.Pending
            };
            
            var submissions = new List<Submission> { mockSubmission };
            _contextMock.Setup(c => c.Submissions).ReturnsDbSet(submissions);

            // Act
            int grade = 95;
            string comment = "Excellent work!";
            var result = await _sut.GradeSubmissionAsync(submissionId, grade, comment, instructorId);

            // Assert
            result.Should().BeTrue();
            mockSubmission.Grade.Should().Be(grade);
            mockSubmission.Comment.Should().Be(comment);
            mockSubmission.Status.Should().Be(SubmissionStatus.Graded);
            
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationServiceMock.Verify(n => n.CreateAndSendToUserAsync(
                studentId, 
                "Assignment Graded", 
                $"Your assignment '{mockAssignment.Title}' in course '{mockCourse.Title}' has been graded.", 
                NotificationType.AssignmentUpdate), Times.Once);
        }

        [Fact]
        public async Task CreateCourseAsync_ValidInput_ShouldSaveAndNotifyStudents()
        {
            // Arrange
            string instructorId = "instructor1";
            var model = new CreateCourseViewModel 
            { 
                Title = "ASP.NET Core Web API", 
                Description = "Learn how to build RESTful APIs",
                Price = 199.99m 
            };
            
            var courses = new List<Course>();
            _contextMock.Setup(c => c.Courses).ReturnsDbSet(courses);
            
            var subscribers = new List<NewsletterSubscriber>(); // Empty for this test
            _contextMock.Setup(c => c.NewsletterSubscribers).ReturnsDbSet(subscribers);

            _contextMock.Setup(c => c.Courses.Add(It.IsAny<Course>())).Callback<Course>(courses.Add);

            // Act
            var result = await _sut.CreateCourseAsync(model, instructorId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(model.Title);
            result.InstructorId.Should().Be(instructorId);
            
            _contextMock.Verify(c => c.Courses.Add(It.IsAny<Course>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            _notificationServiceMock.Verify(n => n.CreateAndSendToRoleAsync(
                "Student", 
                "New Course Available", 
                $"A new course '{model.Title}' has been published.", 
                NotificationType.SystemAlert), Times.Once);
        }
    }
}
