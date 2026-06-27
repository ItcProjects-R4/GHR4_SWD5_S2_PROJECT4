using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.Domain.Models;
using LMS.Domain.ViewModels;
using LMS.Domain.ViewModels.Home;
using LMS.Domain.ViewModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AutoMapper;


namespace LMS.PL.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICourseService _courseService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HomeController(
            ICourseService courseService,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _courseService = courseService;
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Instructor"))
                {
                    return RedirectToAction("Dashboard", "Instructor");
                }
                else if (User.IsInRole("Assistant"))
                {
                    return RedirectToAction("Dashboard", "Assistant");
                }
            }
            var featured = await _courseService.GetFeaturedCoursesAsync(3);
            var viewModels = _mapper.Map<List<CourseViewModel>>(featured);
            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

            var instructor = instructors.FirstOrDefault(i => i.Email == "kk@gmail.com") ?? instructors.FirstOrDefault();

            var model = new AboutViewModel
            {
                InstructorName = instructor != null
                    ? $"{instructor.FirstName} {instructor.LastName}"
                    : "Instructor",

                Biography = !string.IsNullOrWhiteSpace(instructor?.Biography)
                    ? instructor.Biography
                    : "No biography available.",

                AvatarUrl = !string.IsNullOrWhiteSpace(instructor?.AvatarUrl)
                    ? instructor.AvatarUrl
                    : "/images/default-avatar.png",

                CoursesCount = await _context.Courses.CountAsync(),

                StudentsCount = (await _userManager.GetUsersInRoleAsync("Student")).Count
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var subject = $"Contact Message: {model.Subject}";

            var body = $@"
                 <h3>New Contact Message</h3>
                 <p><strong>Name:</strong> {model.FirstName} {model.LastName}</p>
                 <p><strong>Email:</strong> {model.Email}</p>
                 <p><strong>Subject:</strong> {model.Subject}</p>
                 <p><strong>Message:</strong> {model.Message}</p>
                 <hr/>
                 <p><strong>Reply to:</strong> {model.Email}</p>
            ";

            await _emailSender.SendEmailAsync(
                "amira@gmail.com",
                subject,
                body
            );

            TempData["SuccessMessage"] = "Your message has been sent successfully.";

            return RedirectToAction("Contact");
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
        public IActionResult Subscribe()
        {
           
            return View();
        }
        
    }
}


