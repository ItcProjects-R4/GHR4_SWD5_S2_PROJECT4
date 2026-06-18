
namespace LMS.BLL.ViewModels.Student.CourseDetails
{
    public class CourseDetailsViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<ModuleViewModel> Modules { get; set; } = [];
    }
}