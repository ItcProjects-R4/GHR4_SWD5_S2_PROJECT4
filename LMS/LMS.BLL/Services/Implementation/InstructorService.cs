using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels.Instructor.Enrollments;
using LMS.Domain.ViewModels.Instructor.CourseDetails;
using LMS.Domain.ViewModels.Student.CourseDetails;
using LMS.DAL.Data;
using Microsoft.EntityFrameworkCore;
using LMS.Domain.Enums;

namespace LMS.BLL.Services.Implementation
{
    public class InstructorService(IApplicationDbContext context, ICurrentUserService currentUserService)
        : IInstructorService
    {
        public async Task<List<CourseEnrollmentGroupViewModel>> GetEnrollmentsAsync(string search)
        {
            var instructorId = currentUserService.UserId;

            var instructorEnrollments = await context.Enrollments
                .AsNoTracking()
                .Where(e => e.Course.InstructorId == instructorId &&
                (string.IsNullOrEmpty(search) ||
                e.Student.FirstName.Contains(search.Trim()) ||
                e.Student.LastName.Contains(search.Trim()) ||
                (e.Student.FirstName + " " + e.Student.LastName).Contains(search.Trim()) ||
                e.Course.Title.Contains(search.Trim())))
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new InstructorEnrollmentViewModel
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentFirstName = e.Student.FirstName,
                    StudentLastName = e.Student.LastName,
                    StudentAvatarUrl = e.Student.AvatarUrl,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status.ToString(),
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    CompletedLessonsCount = e.CompletedLessonsCount,
                    TotalLessonsCount = e.Course.TotalLessonCount
                })
                .ToListAsync();


            var groupedEnrollments = instructorEnrollments
                .GroupBy(e => new { e.CourseId, e.CourseTitle })
                .Select(g => new CourseEnrollmentGroupViewModel
                {
                    CourseId = g.Key.CourseId,
                    CourseTitle = g.Key.CourseTitle,
                    Enrollments = g.ToList()
                })
                .OrderBy(g => g.CourseTitle)
                .ToList();

            return groupedEnrollments;
        }

        public async Task<InstructorCourseDetailsPageViewModel> GetCourseDetailsPageAsync(int courseId)
        {
            var instructorId = currentUserService.UserId;

            if (courseId <= 0)
                throw new ArgumentException("Course ID must be greater than zero.", nameof(courseId));

            var courseDetails = await context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId && c.InstructorId == instructorId)
                .Select(c => new CourseDetailsViewModel
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Price = c.Price,

                    Modules = c.Modules
                        .OrderBy(module => module.OrderIndex)
                        .Select(module => new ModuleViewModel
                        {
                            Id = module.Id,
                            Title = module.Title,

                            Contents = module.Contents
                                .OrderBy(content => content.OrderIndex)
                                .Select(content => new ContentViewModel
                                {
                                    Id = content.Id,
                                    Title = content.Title,
                                }).ToList(),

                            Assignment = module.Assignment == null ? null : new AssignmentViewModel
                            {
                                Id = module.Assignment.Id,
                                Title = module.Assignment.Title,
                                DueDate = module.Assignment.DueDate,
                                MaxScore = module.Assignment.MaxScore
                            }

                        }).ToList()
                }).FirstOrDefaultAsync();

            if (courseDetails == null)
                throw new Exception("Course not found or you are not authorized to view it.");

            var enrollments = await context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Select(e => new
                {
                    e.Status,
                    e.CompletedLessonsCount
                })
                .ToListAsync();

            int enrolledStudentsCount = enrollments.Count;
            int completedStudentsCount = enrollments.Count(e => e.Status == EnrollmentStatus.Completed);

            int averageProgressPercent = 0;
            int totalLessons = courseDetails.Modules.Sum(m => m.Contents.Count);

            if (enrolledStudentsCount > 0 && totalLessons > 0)
            {
                int sumProgress = 0;
                foreach (var enrollment in enrollments)
                {
                    int progress = (int)enrollment.CompletedLessonsCount * 100 / totalLessons;
                    sumProgress += progress;
                }

                averageProgressPercent = (int)Math.Round((double)sumProgress / enrolledStudentsCount);
            }

            int? activeContentId = courseDetails.Modules.SelectMany(m => m.Contents).FirstOrDefault()?.Id;
            ContentViewModel? activeContent = null;
            if (activeContentId.HasValue)
            {
                activeContent = await GetContentAsync(activeContentId.Value);
            }

            return new InstructorCourseDetailsPageViewModel
            {
                Course = courseDetails,
                ActiveContentId = activeContentId,
                ActiveContent = activeContent,
                TotalContents = totalLessons,
                TotalModules = courseDetails.Modules.Count,
                EnrolledStudentsCount = enrolledStudentsCount,
                CompletedStudentsCount = completedStudentsCount,
                AverageProgressPercent = averageProgressPercent
            };
        }

        public async Task<ContentViewModel> GetContentAsync(int contentId)
        {
            var instructorId = currentUserService.UserId;

            if (contentId <= 0)
                throw new ArgumentException("Content ID must be greater than zero.", nameof(contentId));

            var content = await context.Contents
                .AsNoTracking()
                .Where(c => c.Id == contentId && c.Module.Course.InstructorId == instructorId)
                .Select(c => new ContentViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    VideoUrl = c.VideoUrl,
                    ArticleUrl = c.ArticleUrl,
                    Text = c.Text,
                    CourseId = c.Module.CourseId,
                }).FirstOrDefaultAsync();

            if (content == null)
                throw new Exception("Content not found");

            return content;
        }

        public async Task<InstructorAssignmentDetailsViewModel> GetAssignmentDetailsAsync(int assignmentId)
        {
            if (assignmentId <= 0)
                throw new ArgumentException("Assignment ID must be greater than zero.", nameof(assignmentId));

            var instructorId = currentUserService.UserId;

            var assignment = await context.Assignments
                .AsNoTracking()
                .Where(a => a.Id == assignmentId && a.Module.Course.InstructorId == instructorId)
                .Select(a => new InstructorAssignmentDetailsViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    FileUrl = a.FileUrl,
                    DueDate = a.DueDate,
                    MaxScore = a.MaxScore
                }).FirstOrDefaultAsync();

            if (assignment == null)
                throw new Exception("Assignment not found");

            var submissions = await context.Submissions
                .AsNoTracking()
                .Where(s => s.AssignmentId == assignmentId)
                .Select(s => new InstructorSubmissionViewModel
                {
                    Id = s.Id,
                    SubmittedAt = s.SubmittedAt,
                    UpdatedAt = s.UpdatedAt,
                    StudentId = s.StudentId,
                    StudentFullName = $"{s.Student.FirstName} {s.Student.LastName}",
                    StudentAvatarUrl = s.Student.AvatarUrl,
                    Grade = s.Grade,
                    SubmissionStatus = s.Status.ToString(),
                    Comment = s.Comment,
                    SubmissionFiles = s.SubmissionFiles
                        .Select(f => new SubmissionFileViewModel
                        {
                            Id = f.Id,
                            FileUrl = f.FileUrl,
                            FileName = f.FileName,
                            FileType = f.FileType,
                            FileSize = f.FileSize
                        }).ToList()
                })
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            assignment.Submissions = submissions;
            return assignment;
        }
    }
}