using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Implementation
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ApplicationDbContext _context;

        public SubmissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetFilteredSubmissionsAsync(string searchString, string statusFilter)
        {
            var query = _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && !statusFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (statusFilter.Equals("pending", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Pending);
                }
                else if (statusFilter.Equals("graded", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Graded);
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToLower();
                query = query.Where(s =>
                    (s.Student.FirstName != null && s.Student.FirstName.ToLower().Contains(search)) ||
                    (s.Student.LastName != null && s.Student.LastName.ToLower().Contains(search)) ||
                    (s.Assignment.Title != null && s.Assignment.Title.ToLower().Contains(search))
                );
            }

            return await query.OrderByDescending(s => s.SubmittedAt).ToListAsync();
        }

        public async Task<int> GetPendingSubmissionsCountAsync()
        {
            return await _context.Submissions.CountAsync(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Pending);
        }

        public async Task<int> GetGradedTodayCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Submissions.CountAsync(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Graded && s.UpdatedAt >= today);
        }

        public async Task<IEnumerable<Submission>> GetRecentSubmissionsAsync(int count)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Submission> GetSubmissionByIdAsync(int id)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> GradeSubmissionAsync(int id, int grade, string feedback)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null) return false;

            submission.Grade = grade;
            submission.Comment = feedback;
            submission.Status = LMS.Domain.Enums.SubmissionStatus.Graded;
            submission.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

    }

}



//using LMS.BLL.Services.Interfaces;
//using LMS.DAL.Data;
//using LMS.Domain.Models;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LMS.BLL.Services.Implementation
//{
//    public class SubmissionService : ISubmissionService
//    {
//        private readonly ApplicationDbContext _context;

//        public SubmissionService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ── SEED DATA FOR TESTING ──────────────────────────────
//        private List<Submission> GetSeedData()
//        {
//            return new List<Submission>
//            {
//                new Submission
//                {
//                    Id = 1,
//                    Student = new ApplicationUser { FirstName = "Maya", LastName = "Chen", Email = "maya@example.com" },
//                    Assignment = new Assignment { Title = "Full-Stack Web Development" },
//                    SubmittedAt = DateTime.Now.AddHours(-2),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Pending,
//                    Grade = null,
//                    Comment = null
//                },
//                new Submission
//                {
//                    Id = 2,
//                    Student = new ApplicationUser { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
//                    Assignment = new Assignment { Title = "UI/UX Design Masterclass" },
//                    SubmittedAt = DateTime.Now.AddHours(-5),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Pending,
//                    Grade = null,
//                    Comment = null
//                },
//                new Submission
//                {
//                    Id = 3,
//                    Student = new ApplicationUser { FirstName = "Sarah", LastName = "Miller", Email = "sarah@example.com" },
//                    Assignment = new Assignment { Title = "Python for Data Analysis" },
//                    SubmittedAt = DateTime.Now.AddDays(-1),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Pending,
//                    Grade = null,
//                    Comment = null
//                },
//                new Submission
//                {
//                    Id = 4,
//                    Student = new ApplicationUser { FirstName = "David", LastName = "Clark", Email = "david@example.com" },
//                    Assignment = new Assignment { Title = "Digital Marketing Strategy 2026" },
//                    SubmittedAt = DateTime.Now.AddDays(-2),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Pending,
//                    Grade = null,
//                    Comment = null
//                },
//                new Submission
//                {
//                    Id = 5,
//                    Student = new ApplicationUser { FirstName = "Michael", LastName = "Kane", Email = "michael@example.com" },
//                    Assignment = new Assignment { Title = "Web Design Fundamentals" },
//                    SubmittedAt = DateTime.Now.AddDays(-1),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Graded,
//                    Grade = 92,
//                    Comment = "Excellent work on the semantic HTML structure! The tags like header, main, and footer are all aligned correctly.",
//                    UpdatedAt = DateTime.Now.AddDays(-1)
//                },
//                new Submission
//                {
//                    Id = 6,
//                    Student = new ApplicationUser { FirstName = "Emily", LastName = "Davis", Email = "emily@example.com" },
//                    Assignment = new Assignment { Title = "Full-Stack Web Development" },
//                    SubmittedAt = DateTime.Now.AddDays(-3),
//                    Status = LMS.Domain.Enums.SubmissionStatus.Graded,
//                    Grade = 88,
//                    Comment = "Strong execution of responsive layout styling. CSS layout grids and flexboxes were integrated in a professional modular format. Keep it up!",
//                    UpdatedAt = DateTime.Now.AddDays(-3)
//                }
//            };
//        }
//        // ─────────────────────────────────────────────────────────

//        public async Task<IEnumerable<Submission>> GetFilteredSubmissionsAsync(string searchString, string statusFilter)
//        {
//            var submissions = GetSeedData().AsQueryable();

//            if (!string.IsNullOrEmpty(statusFilter) && !statusFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
//            {
//                if (statusFilter.Equals("pending", StringComparison.OrdinalIgnoreCase))
//                {
//                    submissions = submissions.Where(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Pending);
//                }
//                else if (statusFilter.Equals("graded", StringComparison.OrdinalIgnoreCase))
//                {
//                    submissions = submissions.Where(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Graded);
//                }
//            }

//            if (!string.IsNullOrEmpty(searchString))
//            {
//                var search = searchString.ToLower();
//                submissions = submissions.Where(s =>
//                    (s.Student.FirstName != null && s.Student.FirstName.ToLower().Contains(search)) ||
//                    (s.Student.LastName != null && s.Student.LastName.ToLower().Contains(search)) ||
//                    (s.Assignment.Title != null && s.Assignment.Title.ToLower().Contains(search))
//                );
//            }

//            return await Task.FromResult(submissions.OrderByDescending(s => s.SubmittedAt).ToList());
//        }

//        public async Task<int> GetPendingSubmissionsCountAsync()
//        {
//            var count = GetSeedData().Count(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Pending);
//            return await Task.FromResult(count);
//        }

//        public async Task<int> GetGradedTodayCountAsync()
//        {
//            var today = DateTime.Today;
//            var count = GetSeedData().Count(s => s.Status == LMS.Domain.Enums.SubmissionStatus.Graded && s.UpdatedAt >= today);
//            return await Task.FromResult(count);
//        }

//        public async Task<IEnumerable<Submission>> GetRecentSubmissionsAsync(int count)
//        {
//            var submissions = GetSeedData()
//                .OrderByDescending(s => s.SubmittedAt)
//                .Take(count)
//                .ToList();
//            return await Task.FromResult(submissions);
//        }

//        public async Task<Submission> GetSubmissionByIdAsync(int id)
//        {
//            var submission = GetSeedData().FirstOrDefault(s => s.Id == id);
//            return await Task.FromResult(submission);
//        }

//        public async Task<bool> GradeSubmissionAsync(int id, int grade, string feedback)
//        {
//            // In real implementation, save to database
//            return await Task.FromResult(true);
//        }
//    }
//}