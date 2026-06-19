using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LMS.PL.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICourseService _courseService;

        public HomeController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var featured = await _courseService.GetFeaturedCoursesAsync(3);
            return View(featured);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Simulate message delivery
            TempData["SuccessMessage"] = "Thank you! Your message has been sent successfully.";
            return RedirectToAction(nameof(Contact));
        }
       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult TestLayout()
        {
           
            return View();
        }
        //subscribing to newsletter
        [HttpGet]
        public IActionResult Subscribe()
        {
           
            return View();
        }
        

    }
}
