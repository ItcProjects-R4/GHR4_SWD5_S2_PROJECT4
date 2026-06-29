using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using Microsoft.EntityFrameworkCore;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using LMS.Domain.ViewModels.Instructor.CourseDetails;
using Microsoft.AspNetCore.Http;
using LMS.Domain.ViewModels.Instructor.Enrollments;
using LMS.Domain.ViewModels.Student.CourseDetails;
using LMS.Domain.ViewModels.Instructor.Dashboard;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace LMS.BLL.Services.Implementation
{
    public class InstructorService(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ICloudinaryService cloudinaryService,
        IEmailSender emailSender,
        INotificationService notificationService)
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
                                    ArticleUrl = content.ArticleUrl
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
                    ModuleTitle = c.Module.Title,
                    ModuleOrderIndex = c.Module.OrderIndex,
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
                        .Select(f => new Domain.ViewModels.Student.CourseDetails.SubmissionFileViewModel
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

        public async Task<List<Course>> GetInstructorCoursesAsync(string instructorId, string? searchString, string? sortBy)
        {
            var query = context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Modules)
                .Include(c => c.Enrollments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.Trim().ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(search) || c.Description.ToLower().Contains(search));
            }

            query = sortBy switch
            {
                "oldest" => query.OrderBy(c => c.Id),
                "title-asc" => query.OrderBy(c => c.Title),
                "title-desc" => query.OrderByDescending(c => c.Title),
                "price-asc" => query.OrderBy(c => c.Price),
                "price-desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderByDescending(c => c.Id)
            };

            return await query.ToListAsync();
        }

        public async Task<Course> CreateCourseAsync(CreateCourseViewModel model, string instructorId)
        {
            // All images are uploaded to Cloudinary; default starts empty (handled by view fallback placeholders)
            string thumbnailUrl = string.Empty;
            if (model.ThumbnailFile != null)
            {
                thumbnailUrl = await cloudinaryService.UploadImageAsync(model.ThumbnailFile);
            }

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                ThumbnailUrl = thumbnailUrl,
                InstructorId = instructorId,
                TotalLessonCount = 0,
                Modules = [],
                Enrollments = [],
                Payments = [],
                Instructor = null!
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            await notificationService.CreateAndSendToRoleAsync("Student", "New Course Available", $"A new course '{course.Title}' has been published.", NotificationType.SystemAlert);

            // email all subscribers about the new course in the background
            _ = Task.Run(async () =>
            {
                try
                {
                    var subscribers = await context.NewsletterSubscribers.Select(s => s.Email).ToListAsync();
                    if (subscribers.Count > 0)
                    {
                        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "NewCourseEmail.html");
                        var templateHtml = await System.IO.File.ReadAllTextAsync(templatePath);
                        var body = templateHtml
                            .Replace("{{CourseTitle}}", course.Title)
                            .Replace("{{CourseDescription}}", course.Description)
                            .Replace("{{CoursePrice}}", course.Price == 0 ? "Free" : $"${course.Price:F2}")
                            .Replace("{{CourseId}}", course.Id.ToString());
                        foreach (var email in subscribers)
                        {
                            await emailSender.SendEmailAsync(email, $"New Course Published: {course.Title}", body);
                        }
                    }
                }
                catch (Exception)
                {
                    // fails silently so course creation doesn't crash if SMTP server is down
                }
            });
            return course;
        }

        public async Task<Course?> GetCourseForEditAsync(int courseId, string instructorId)
        {
            return await context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Contents)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Assignment)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == instructorId);
        }

        public async Task<bool> UpdateCourseAsync(int courseId, CreateCourseViewModel model, string instructorId)
        {
            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == instructorId);
            if (course == null) return false;

            course.Title = model.Title;
            course.Description = model.Description;
            course.Price = model.Price;

            if (model.ThumbnailFile != null)
            {
                course.ThumbnailUrl = await cloudinaryService.UploadImageAsync(model.ThumbnailFile);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int courseId, string instructorId)
        {
            var course = await context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Payments)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Contents)
                        .ThenInclude(c => c.Progresses)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Assignment)
                        .ThenInclude(a => a!.Submissions)
                            .ThenInclude(s => s.SubmissionFiles)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == instructorId);

            if (course == null) return false;

            // 1. Remove related student lesson progress records
            var progresses = course.Modules.SelectMany(m => m.Contents).SelectMany(c => c.Progresses).ToList();
            if (progresses.Any())
            {
                context.Progresses.RemoveRange(progresses);
            }

            // 2. Remove student assignment submission files
            var submissionFiles = course.Modules
                .Where(m => m.Assignment != null)
                .Select(m => m.Assignment!)
                .SelectMany(a => a.Submissions)
                .SelectMany(s => s.SubmissionFiles)
                .ToList();
            if (submissionFiles.Any())
            {
                context.SubmissionFiles.RemoveRange(submissionFiles);
            }

            // 3. Remove student assignment submissions
            var submissions = course.Modules
                .Where(m => m.Assignment != null)
                .Select(m => m.Assignment!)
                .SelectMany(a => a.Submissions)
                .ToList();
            if (submissions.Any())
            {
                context.Submissions.RemoveRange(submissions);
            }

            // 4. Remove assignments
            var assignments = course.Modules
                .Where(m => m.Assignment != null)
                .Select(m => m.Assignment!)
                .ToList();
            if (assignments.Any())
            {
                context.Assignments.RemoveRange(assignments);
            }

            // 5. Remove course contents/lessons
            var contents = course.Modules.SelectMany(m => m.Contents).ToList();
            if (contents.Any())
            {
                context.Contents.RemoveRange(contents);
            }

            // 6. Remove course modules
            if (course.Modules.Any())
            {
                context.Modules.RemoveRange(course.Modules);
            }

            // 7. Remove student enrollments
            if (course.Enrollments.Any())
            {
                context.Enrollments.RemoveRange(course.Enrollments);
            }

            // 8. Remove payment histories
            if (course.Payments.Any())
            {
                context.Payments.RemoveRange(course.Payments);
            }

            // 9. Finally, remove the course itself
            context.Courses.Remove(course);

            await context.SaveChangesAsync();
            return true;
        }


        public async Task<Module> AddModuleAsync(int courseId, string moduleTitle)
        {
            var course = await context.Courses.Include(c => c.Modules).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) throw new ArgumentException("Course not found.");

            var orderIndex = course.Modules.Count + 1;
            var module = new Module
            {
                CourseId = courseId,
                Title = moduleTitle,
                OrderIndex = orderIndex,
                Contents = []
            };

            context.Modules.Add(module);
            await context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteModuleAsync(int moduleId, int courseId, string instructorId)
        {
            var module = await context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == moduleId && m.Course.Id == courseId && m.Course.InstructorId == instructorId);
            if (module == null) return false;

            context.Modules.Remove(module);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Content> AddContentAsync(int moduleId, CreateContentViewModel model)
        {
            var module = await context.Modules.Include(m => m.Contents).FirstOrDefaultAsync(m => m.Id == moduleId);
            if (module == null) throw new ArgumentException("Module not found.");

            string? videoUrl = null;
            if (model.ContentType == "video" && model.VideoFile != null)
            {
                videoUrl = await cloudinaryService.UploadVideoAsync(model.VideoFile);
            }
            else if (model.ContentType == "link")
            {
                videoUrl = model.VideoUrl;
            }

            var orderIndex = module.Contents.Count + 1;
            var content = new Content
            {
                ModuleId = moduleId,
                Title = model.Title,
                OrderIndex = orderIndex,
                VideoUrl = videoUrl,
                ArticleUrl = model.ArticleUrl,
                Text = model.Text,
                Progresses = []
            };

            context.Contents.Add(content);

            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == module.CourseId);
            if (course != null)
            {
                course.TotalLessonCount++;
            }

            await context.SaveChangesAsync();
            return content;
        }

        public async Task<bool> DeleteContentAsync(int contentId, int courseId, string instructorId)
        {
            var content = await context.Contents
                .Include(c => c.Module)
                    .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(c => c.Id == contentId && c.Module.Course.Id == courseId && c.Module.Course.InstructorId == instructorId);

            if (content == null) return false;

            context.Contents.Remove(content);

            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course != null && course.TotalLessonCount > 0)
            {
                course.TotalLessonCount--;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Assignment> AddAssignmentAsync(int moduleId, string title, DateTime dueDate, int maxScore, IFormFile? resourceFile)
        {
            string fileUrl = string.Empty;
            if (resourceFile != null)
            {
                fileUrl = await cloudinaryService.UploadFileAsync(resourceFile);
            }

            var assignment = new Assignment
            {
                ModuleId = moduleId,
                Title = title,
                FileUrl = fileUrl,
                DueDate = dueDate,
                MaxScore = maxScore,
                Submissions = []
            };

            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int assignmentId, int courseId, string instructorId)
        {
            var assignment = await context.Assignments
                .Include(a => a.Module)
                    .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(a => a.Id == assignmentId && a.Module.Course.Id == courseId && a.Module.Course.InstructorId == instructorId);

            if (assignment == null) return false;

            context.Assignments.Remove(assignment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Submission>> GetSubmissionsQueueAsync(string instructorId, string? searchString, string? statusFilter)
        {
            var query = context.Submissions
                .Where(s => s.Assignment.Module.Course.InstructorId == instructorId)
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Module)
                        .ThenInclude(m => m.Course)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.Trim().ToLower();
                query = query.Where(s =>
                    (s.Student.FirstName + " " + s.Student.LastName).ToLower().Contains(search) ||
                    s.Assignment.Module.Course.Title.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
            {
                if (statusFilter == "pending")
                    query = query.Where(s => s.Status == SubmissionStatus.Pending);
                else if (statusFilter == "graded")
                    query = query.Where(s => s.Status == SubmissionStatus.Graded);
            }

            return await query.OrderByDescending(s => s.SubmittedAt).ToListAsync();
        }

        public async Task<Submission?> GetSubmissionForGradingAsync(int submissionId, string instructorId)
        {
            return await context.Submissions
                .Include(s => s.Student)
                .Include(s => s.SubmissionFiles)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Module)
                        .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(s => s.Id == submissionId && s.Assignment.Module.Course.InstructorId == instructorId);
        }

        public async Task<bool> GradeSubmissionAsync(int submissionId, int grade, string? comment, string instructorId)
        {
            var submission = await context.Submissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Module)
                        .ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(s => s.Id == submissionId && s.Assignment.Module.Course.InstructorId == instructorId);

            if (submission == null) return false;

            submission.Grade = grade;
            submission.Comment = comment;
            submission.Status = SubmissionStatus.Graded;
            submission.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            await notificationService.CreateAndSendToUserAsync(
                submission.StudentId, 
                "Assignment Graded", 
                $"Your assignment '{submission.Assignment.Title}' in course '{submission.Assignment.Module.Course.Title}' has been graded.", 
                NotificationType.AssignmentUpdate);

            return true;
        }
    

        public async Task<InstructorDashboardViewModel> GetInstructorDashboardAsync()
        {
            var instructorId = currentUserService.UserId;

            var dashboard = new InstructorDashboardViewModel();

            var instructorCourses = context.Courses.Where(c => c.InstructorId == instructorId);
            dashboard.ActiveCourses = await instructorCourses.CountAsync();

            var instructorEnrollments = context.Enrollments.Where(e => e.Course.InstructorId == instructorId);
            dashboard.TotalEnrollments = await instructorEnrollments.CountAsync();

            dashboard.TotalUsers = await instructorEnrollments.Select(e => e.StudentId).Distinct().CountAsync();

            dashboard.TotalRevenue = await context.Payments
                .Where(p => p.Course.InstructorId == instructorId && p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount);

            var colors = new[] { "bg-primary", "bg-success", "bg-info", "bg-warning" };
            var random = new Random();

            var recentEnrollments = await context.Payments
                .Include(p => p.Student)
                .Include(p => p.Course)
                .Where(p => p.Course.InstructorId == instructorId && p.Status == PaymentStatus.Completed)
                .OrderByDescending(p => p.PaidAt)
                .Take(5)
                .ToListAsync();

            dashboard.RecentEnrollments = recentEnrollments.Select(p => new RecentEnrollmentViewModel
            {
                StudentName = $"{p.Student.FirstName} {p.Student.LastName}",
                StudentInitials = $"{p.Student.FirstName?.FirstOrDefault()}{p.Student.LastName?.FirstOrDefault()}",
                CourseTitle = p.Course.Title,
                Amount = p.Amount,
                EnrolledAt = p.PaidAt,
                AvatarBgColor = colors[random.Next(colors.Length)]
            }).ToList();

            var recentSubmissions = await context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .ThenInclude(a => a.Module)
                .ThenInclude(m => m.Course)
                .Where(s => s.Assignment.Module.Course.InstructorId == instructorId)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(5)
                .ToListAsync();

            dashboard.RecentSubmissions = recentSubmissions.Select(s => new RecentSubmissionViewModel
            {
                StudentName = $"{s.Student.FirstName} {s.Student.LastName}",
                StudentInitials = $"{s.Student.FirstName?.FirstOrDefault()}{s.Student.LastName?.FirstOrDefault()}",
                CourseAndModule = $"{s.Assignment.Module.Course.Title} • {s.Assignment.Module.Title}",
                Status = s.Status.ToString(),
                AvatarBgColor = colors[random.Next(colors.Length)]
            }).ToList();

            return dashboard;
        }
    }
}