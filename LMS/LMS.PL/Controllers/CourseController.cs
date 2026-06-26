using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace LMS.PL.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Browse(string? searchString, string typeFilter = "all", string sortOrder = "newest")
        {
            var courses = await _courseService.GetFilteredCoursesAsync(searchString, typeFilter, sortOrder);

            var viewModel = new BrowseCoursesViewModel
            {
                Courses = courses,
                SearchString = searchString,
                TypeFilter = typeFilter,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();

            return View(course);
        }
    }
}
