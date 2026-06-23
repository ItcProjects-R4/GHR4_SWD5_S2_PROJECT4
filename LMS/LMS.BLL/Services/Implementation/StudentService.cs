using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels.Student.CourseDetails;
using LMS.Domain.ViewModels.Student.Dashboard;
using LMS.BLL.DTOS;
using LMS.DAL.Data;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.BLL.Services.Implementation
{
    public class StudentService(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
       UserManager<ApplicationUser> userManager, 
       ICloudinaryService cloudinaryService,
       ICheckoutService checkoutService
       )
       : IStudentService
    {
        public async Task<IEnumerable<ApplicationUser>> GetFilteredUsersAsync(string searchString, string roleFilter)
        {
            IList<ApplicationUser> users;

            if (!string.IsNullOrEmpty(roleFilter) && !roleFilter.Equals("All Roles", StringComparison.OrdinalIgnoreCase))
            {
                users = await userManager.GetUsersInRoleAsync(roleFilter);
            }
            else
            {
                var students = await userManager.GetUsersInRoleAsync("Student");
                var assistants = await userManager.GetUsersInRoleAsync("Assistant");
                users = students.Concat(assistants).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var query = searchString.ToLower();
                users = users.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(query)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(query)) ||
                    (u.Email != null && u.Email.ToLower().Contains(query)) ||
                    u.Id.ToLower().Contains(query)
                ).ToList();
            }

            return users;
        }


        public async Task<StudentDashboardViewModel> GetStudentDashboardAsync()
        {
            string studentId = currentUserService.UserId;

            var studentDashboard = await context.Users
                .AsNoTracking()
                .Where(stud => stud.Id == studentId)
                .Select(stud => new StudentDashboardViewModel
                {
                    FirstName = stud.FirstName,
                    EnrolledCoursesCount = stud.Enrollments.Count(),
                    ActiveCoursesCount = stud.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Active),
                    CompletedCoursesCount = stud.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Completed),

                    ContinueLearningCourses = stud.Enrollments
                        .Where(enrollment => enrollment.Status == EnrollmentStatus.Active)
                        .OrderByDescending(enrollment => enrollment.EnrolledAt)
                        .Select(enrollment => new ContinueLearningCourseViewModels
                        {
                            CourseId = enrollment.CourseId,
                            CourseTitle = enrollment.Course.Title,
                            ThumbnailUrl = enrollment.Course.ThumbnailUrl,
                            TotalLessonsCount = enrollment.Course.TotalLessonCount,
                            CompletedLessonsCount = enrollment.CompletedLessonsCount,
                            ProgressPercentage = (enrollment.Course.TotalLessonCount > 0)
                                ? (int)Math.Round((double)enrollment.CompletedLessonsCount * 100 / enrollment.Course.TotalLessonCount)
                                : 0
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (studentDashboard == null)
                throw new Exception("Student not found");

            studentDashboard.RecentlyAddedCourses = await context.Courses
              .AsNoTracking()
              .Where(c => !context.Enrollments
              .Any(e => e.StudentId == studentId && e.CourseId == c.Id))
              .OrderByDescending(c => c.Id)
              .Take(3)
              .Select(c => new BrowseCourseViewModel
              {
                  CourseId = c.Id,
                  CourseTitle = c.Title,
                  ThumbnailUrl = c.ThumbnailUrl,
                  Price = c.Price
              }).ToListAsync();

            return studentDashboard;
        }

        public async Task<List<EnrolledCoursesViewModel>> GetEnrolledCoursesAsync(string status, string search)
        {
            string studentId = currentUserService.UserId;

            var enrolledCourses = await context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId &&
                            (string.IsNullOrEmpty(status) || e.Status.ToString() == status) &&
                            (string.IsNullOrEmpty(search) || e.Course.Title.Contains(search.Trim())))
                .Select(e => new EnrolledCoursesViewModel
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    ThumbnailUrl = e.Course.ThumbnailUrl,
                    EnrollmentStatus = e.Status.ToString(),
                    TotalLessonsCount = e.Course.TotalLessonCount,
                    CompletedLessonsCount = e.CompletedLessonsCount,
                    ProgressPercentage = (e.Course.TotalLessonCount > 0) ?
                                         ((int)Math.Round((double)e.CompletedLessonsCount * 100 / e.Course.TotalLessonCount))
                                         : 0
                }).ToListAsync();

            return enrolledCourses;
        }

        public async Task<List<BrowseCourseViewModel>> GetBrowseCoursesAsync()
        {
            string studentId = currentUserService.UserId;

            var browseCourses = await context.Courses
                .AsNoTracking()
                .Where(c => !c.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == c.Id))
                .OrderBy(c => c.Title)
                .Select(c => new BrowseCourseViewModel
                {
                    CourseId = c.Id,
                    CourseTitle = c.Title,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Price = c.Price
                }).ToListAsync();

            return browseCourses;
        }

        public async Task<CheckoutResponse> EnrollCourseAsync(int courseId)
        {
            if (courseId <= 0)
                return new CheckoutResponse { Success = false, ErrorMessage = "Invalid course selection." };

            var studentId = currentUserService.UserId;

            var courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
                return new CheckoutResponse { Success = false, ErrorMessage = "Course not found." };

            var isEnrolled = await context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (isEnrolled)
                return new CheckoutResponse { Success = false, ErrorMessage = "You are already enrolled in this course." };

            var user = await userManager.FindByIdAsync(studentId);
            if (user == null)
                return new CheckoutResponse { Success = false, ErrorMessage = "User not found." };

            var email = user.Email;
            var name = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = user.UserName ?? "Student";
            }

            return await checkoutService.InitiateCheckoutAsync(courseId, studentId, email, name);
        }

        public async Task<CourseDetailsPageViewModel> GetCourseDetailsPageAsync(int courseId)
        {
            string studentId = currentUserService.UserId;

            if (courseId <= 0)
                throw new ArgumentException("Course ID must be greater than zero.", nameof(courseId));

            bool isEnrolled = await context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            var courseDetails = await GetCourseDetailsAsync(courseId);

            int? activeContentId = null;
            ContentViewModel? activeContent = null;
            if (isEnrolled)
            {
                activeContentId = GetActiveContentId(courseDetails);
                if (activeContentId.HasValue)
                    activeContent = await GetContentAsync(activeContentId.Value);
            }

            int totalContents = courseDetails.Modules.Sum(m => m.Contents.Count);
            int completedContents = isEnrolled ? courseDetails.Modules.Sum(m => m.Contents.Count(c => c.IsCompleted)) : 0;
            int totalModules = courseDetails.Modules.Count;
            int progressPercent = (isEnrolled && totalContents > 0) ? (int)Math.Round((double)completedContents * 100 / totalContents) : 0;

            return new CourseDetailsPageViewModel
            {
                Course = courseDetails,
                ActiveContentId = activeContentId ?? 0,
                ActiveContent = activeContent,
                TotalContents = totalContents,
                CompletedContents = completedContents,
                TotalModules = totalModules,
                ProgressPercent = progressPercent,
                IsEnrolled = isEnrolled
            };
        }

        public async Task<ContentViewModel> GetContentAsync(int contentId)
        {
            if (contentId <= 0)
                throw new ArgumentException("Content ID must be greater than zero.", nameof(contentId));

            var studentId = currentUserService.UserId;

            var content = await context.Contents
                .AsNoTracking()
                .Where(c => c.Id == contentId && c.Module.Course.Enrollments.Any(e => e.StudentId == studentId))
                .Select(c => new ContentViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    VideoUrl = c.VideoUrl,
                    ArticleUrl = c.ArticleUrl,
                    Text = c.Text,
                    CourseId = c.Module.CourseId,
                    IsCompleted = context.Progresses.Any(p => p.ContentId == c.Id &&
                                                             p.StudentId == studentId &&
                                                             p.IsCompleted)
                }).FirstOrDefaultAsync();

            if (content == null)
                throw new Exception("Content not found");

            return content;
        }

        public async Task<bool> MarkContentAsCompletedAsync(int contentId, int courseId)
        {
            string studentId = currentUserService.UserId;

            if (contentId <= 0 || courseId <= 0)
                return false;

            bool contentExistsInCourse = await context.Contents
                .AnyAsync(c => c.Id == contentId && c.Module.CourseId == courseId);

            if (!contentExistsInCourse)
                return false;

            var enrollmentData = await context.Enrollments
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .Select(e => new
                {
                    Enrollment = e,
                    TotalLessonCount = e.Course.TotalLessonCount
                }).FirstOrDefaultAsync();

            if (enrollmentData == null || enrollmentData.Enrollment.Status != EnrollmentStatus.Active)
                return false;


            var progress = await context.Progresses
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.ContentId == contentId);

            if (progress != null && progress.IsCompleted)
                return true;

            if (progress == null)
            {
                progress = new Progress
                {
                    StudentId = studentId,
                    ContentId = contentId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };

                await context.Progresses.AddAsync(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
            }

            var enrollment = enrollmentData.Enrollment;

            enrollment.CompletedLessonsCount++;

            if (enrollment.CompletedLessonsCount >= enrollmentData.TotalLessonCount)
                enrollment.Status = EnrollmentStatus.Completed;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<AssignmentViewModel> GetAssignmentDetailsAsync(int assignmentId)
        {
            if (assignmentId <= 0)
                throw new ArgumentException("Assignment ID must be greater than zero.", nameof(assignmentId));

            var studentId = currentUserService.UserId;

            var assignment = await context.Assignments
                .AsNoTracking()
                .Where(a => a.Id == assignmentId && a.Module.Course.Enrollments.Any(e => e.StudentId == studentId))
                .Select(a => new AssignmentViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    FileUrl = a.FileUrl,
                    DueDate = a.DueDate,
                    MaxScore = a.MaxScore,

                    Submission = a.Submissions
                        .Where(s => s.StudentId == studentId)
                        .Select(s => new SubmissionViewModel
                        {
                            Id = s.Id,
                            SubmittedAt = s.SubmittedAt,
                            UpdatedAt = s.UpdatedAt,
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
                        }).FirstOrDefault()
                }).FirstOrDefaultAsync();

            if (assignment == null)
                throw new Exception("Assignment not found");

            return assignment;
        }

        public async Task<AssignmentViewModel> SubmitAssignmentAsync(int AssignmentID, List<IFormFile> submissionFiles)
        {
            if (AssignmentID <= 0)
                throw new ArgumentException("Assignment ID must be greater than zero.", nameof(AssignmentID));

            if (submissionFiles == null || submissionFiles.Count == 0)
                throw new ArgumentException("No files were selected for submission.", nameof(submissionFiles));

            const long maxFileSize = 10 * 1024 * 1024;
            var allowedExtensions = new[] { ".pdf", ".docx", ".zip" };

            foreach (var file in submissionFiles)
            {
                if (file.Length > maxFileSize)
                    throw new ArgumentException($"File '{file.FileName}' exceeds the maximum allowed size of 10MB.", nameof(submissionFiles));

                var extension = System.IO.Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    throw new ArgumentException($"File '{file.FileName}' has an invalid extension. Only .pdf, .docx, and .zip files are allowed.", nameof(submissionFiles));
            }

            var studentId = currentUserService.UserId;

            var assignment = await context.Assignments
                .AsNoTracking()
                .Where(a => a.Id == AssignmentID && a.Module.Course.Enrollments.Any(e => e.StudentId == studentId))
                .Select(a => new AssignmentViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    FileUrl = a.FileUrl,
                    DueDate = a.DueDate,
                    MaxScore = a.MaxScore
                }).FirstOrDefaultAsync();

            if (assignment == null)
                throw new Exception("Assignment not found");

            var existingSubmission = await context.Submissions
                .Include(s => s.SubmissionFiles)
                .FirstOrDefaultAsync(s => s.AssignmentId == AssignmentID && s.StudentId == studentId);

            if (existingSubmission != null)
            {
                existingSubmission.SubmittedAt = DateTime.UtcNow;
                existingSubmission.UpdatedAt = DateTime.UtcNow;
                existingSubmission.Status = SubmissionStatus.Pending;

                if (existingSubmission.SubmissionFiles.Count > 0)
                {
                    context.SubmissionFiles.RemoveRange(existingSubmission.SubmissionFiles);
                    existingSubmission.SubmissionFiles.Clear();
                }

                foreach (var file in submissionFiles)
                {
                    var submissionFileUrl = await cloudinaryService.UploadFileAsync(file);

                    existingSubmission.SubmissionFiles.Add(new SubmissionFile
                    {
                        FileUrl = submissionFileUrl,
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = (double)file.Length
                    });
                }
                context.Submissions.Update(existingSubmission);
            }
            else
            {
                var submission = new Submission
                {
                    StudentId = studentId,
                    AssignmentId = AssignmentID,
                    SubmittedAt = DateTime.UtcNow,
                    Status = SubmissionStatus.Pending,
                    SubmissionFiles = new List<SubmissionFile>()
                };

                foreach (var file in submissionFiles)
                {
                    var submissionFileUrl = await cloudinaryService.UploadFileAsync(file);

                    submission.SubmissionFiles.Add(new SubmissionFile
                    {
                        FileUrl = submissionFileUrl,
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = (double)file.Length
                    });
                }

                await context.Submissions.AddAsync(submission);
            }

            await context.SaveChangesAsync();

            return (await GetAssignmentDetailsAsync(AssignmentID))!;
        }

        // Helper Methods
        private async Task<CourseDetailsViewModel> GetCourseDetailsAsync(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("Course ID must be greater than zero.", nameof(courseId));

            string studentId = currentUserService.UserId;

            var courseDetails = await context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => new CourseDetailsViewModel
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Description = c.Description,

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

            var completedContentIds = (await context.Progresses
                .AsNoTracking()
                .Where(p => p.StudentId == studentId && p.IsCompleted && p.Content.Module.CourseId == courseId)
                .Select(p => p.ContentId)
                .ToListAsync())
                .ToHashSet();

            foreach (var module in courseDetails.Modules)
                foreach (var content in module.Contents)
                    content.IsCompleted = completedContentIds.Contains(content.Id);

            return courseDetails;
        }

        private int? GetActiveContentId(CourseDetailsViewModel courseDetails)
        {
            var firstUncompletedContentId = courseDetails.Modules
                .SelectMany(m => m.Contents)
                .FirstOrDefault(c => !c.IsCompleted)?.Id;

            if (firstUncompletedContentId.HasValue)
                return firstUncompletedContentId;

            return courseDetails.Modules
                .SelectMany(m => m.Contents)
                .FirstOrDefault()?.Id;
        }

    }
}