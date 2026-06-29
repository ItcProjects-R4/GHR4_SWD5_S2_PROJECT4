using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace LMS.BLL.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
        private readonly Mock<ICheckoutService> _checkoutServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly StudentService _sut;

        public StudentServiceTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _checkoutServiceMock = new Mock<ICheckoutService>();
            _notificationServiceMock = new Mock<INotificationService>();

            _sut = new StudentService(
                _contextMock.Object,
                _currentUserServiceMock.Object,
                _userManagerMock.Object,
                _cloudinaryServiceMock.Object,
                _checkoutServiceMock.Object,
                _notificationServiceMock.Object
            );
        }

        [Fact]
        public async Task SubmitAssignmentAsync_ValidSubmission_ShouldSaveAndNotifyAssistant()
        {
            // Arrange
            string studentId = "student1";
            int assignmentId = 10;
            string instructorId = "instructor1";
            
            _currentUserServiceMock.Setup(s => s.UserId).Returns(studentId);
            
            var mockStudent = new ApplicationUser { Id = studentId, FirstName = "Test", LastName = "Student" };
            _userManagerMock.Setup(u => u.FindByIdAsync(studentId)).ReturnsAsync(mockStudent);
            
            var mockCourse = new Course 
            { 
                Id = 100, Title = "Unit Testing 101", InstructorId = instructorId, ThumbnailUrl = "", Description = "", Price = 0, Instructor = mockStudent,
                Enrollments = new List<Enrollment> { new Enrollment { StudentId = studentId, CourseId = 100 } }
            };
            var mockModule = new Module { Id = 20, Course = mockCourse, CourseId = 100, Title = "Module 1" };
            var mockAssignment = new Assignment 
            { 
                Id = assignmentId, 
                Title = "Final Project", 
                Module = mockModule,
                ModuleId = 20,
                Submissions = new List<Submission>()
            };

            var assignments = new List<Assignment> { mockAssignment };
            var submissions = new List<Submission>();
            var submissionFiles = new List<SubmissionFile>();

            _contextMock.Setup(c => c.Assignments).ReturnsDbSet(assignments);
            _contextMock.Setup(c => c.Submissions).ReturnsDbSet(submissions);
            _contextMock.Setup(c => c.SubmissionFiles).ReturnsDbSet(submissionFiles);

            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("project.zip");
            formFileMock.Setup(f => f.ContentType).Returns("application/zip");
            formFileMock.Setup(f => f.Length).Returns(1024);
            var files = new List<IFormFile> { formFileMock.Object };

            _cloudinaryServiceMock.Setup(c => c.UploadFileAsync(It.IsAny<IFormFile>())).ReturnsAsync("http://cloudinary/project.zip");

            // Act
            await _sut.SubmitAssignmentAsync(assignmentId, files);

            // Assert
            _contextMock.Verify(c => c.Submissions.AddAsync(It.Is<Submission>(s => 
                s.AssignmentId == assignmentId && 
                s.StudentId == studentId && 
                s.Status == SubmissionStatus.Pending), It.IsAny<CancellationToken>()), Times.Once);

            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _notificationServiceMock.Verify(n => n.CreateAndSendToUserAsync(instructorId, "Assignment Submitted", 
                $"{mockStudent.FirstName} {mockStudent.LastName} has submitted an assignment for '{mockCourse.Title}'.", 
                NotificationType.AssignmentUpdate), Times.Once);

            _notificationServiceMock.Verify(n => n.CreateAndSendToRoleAsync("Assistant", "Assignment Submitted", 
                $"{mockStudent.FirstName} {mockStudent.LastName} has submitted an assignment for '{mockCourse.Title}'.", 
                NotificationType.AssignmentUpdate), Times.Once);
        }

        [Fact]
        public async Task MarkContentAsCompletedAsync_LastLesson_ShouldCompleteEnrollment()
        {
            // Arrange
            string studentId = "student1";
            int contentId = 5;
            int courseId = 100;
            
            _currentUserServiceMock.Setup(s => s.UserId).Returns(studentId);
            
            var mockCourse = new Course { Id = courseId, TotalLessonCount = 2, Title = "", ThumbnailUrl = "", Description = "", Price = 0, InstructorId = "", Instructor = new ApplicationUser { FirstName = "", LastName = "" } };
            var mockModule = new Module { Id = 10, CourseId = courseId, Course = mockCourse, Title = "" };
            var mockContent = new Content { Id = contentId, ModuleId = 10, Module = mockModule, Title = "" };
            
            var contents = new List<Content> { mockContent };
            
            var mockEnrollment = new Enrollment 
            { 
                StudentId = studentId, 
                CourseId = courseId, 
                Status = EnrollmentStatus.Active,
                CompletedLessonsCount = 1,
                Course = mockCourse
            };
            
            var enrollments = new List<Enrollment> { mockEnrollment };
            var progresses = new List<Progress>();

            _contextMock.Setup(c => c.Contents).ReturnsDbSet(contents);
            _contextMock.Setup(c => c.Enrollments).ReturnsDbSet(enrollments);
            _contextMock.Setup(c => c.Progresses).ReturnsDbSet(progresses);
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _sut.MarkContentAsCompletedAsync(contentId, courseId);

            // Assert
            result.Should().BeTrue();
            mockEnrollment.CompletedLessonsCount.Should().Be(2);
            mockEnrollment.Status.Should().Be(EnrollmentStatus.Completed);
            
            _contextMock.Verify(c => c.Progresses.AddAsync(It.Is<Progress>(p => 
                p.StudentId == studentId && 
                p.ContentId == contentId && 
                p.IsCompleted == true), It.IsAny<CancellationToken>()), Times.Once);
                
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
