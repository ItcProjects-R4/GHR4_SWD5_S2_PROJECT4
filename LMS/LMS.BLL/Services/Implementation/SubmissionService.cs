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


