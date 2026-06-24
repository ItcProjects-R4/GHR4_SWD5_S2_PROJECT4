
namespace LMS.Domain.ViewModels.Student.CourseDetails
{
    public class ModuleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<ContentViewModel> Contents { get; set; } = [];
        public AssignmentViewModel? Assignment { get; set; }
    }
}