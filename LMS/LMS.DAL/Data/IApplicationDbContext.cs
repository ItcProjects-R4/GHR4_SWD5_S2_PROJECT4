using LMS.Domain.Models;
using LMS.Domain.Models.LMS.Domain.Models;
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
        DbSet<Notification> Notifications { get; }
        DbSet<Submission> Submissions { get; }
        DbSet<Assignment> Assignments { get; }
        DbSet<SubmissionFile> SubmissionFiles { get; }
        DbSet<ApplicationUser> Users { get; }
        DbSet<NewsletterSubscriber> NewsletterSubscribers { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}