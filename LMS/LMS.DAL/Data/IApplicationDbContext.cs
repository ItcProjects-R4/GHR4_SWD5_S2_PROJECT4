using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.DAL.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Course> Courses { get; }
        DbSet<Module> Modules { get; }
        DbSet<Progress> Progresses { get; }
        DbSet<Content> Contents { get; }
        DbSet<Enrollment> Enrollments { get; }
        DbSet<Payment> Payments { get; }
        DbSet<Submission> Submissions { get; }
        DbSet<Assignment> Assignments { get; }
        DbSet<SubmissionFile> SubmissionFiles { get; }
        DbSet<ApplicationUser> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}