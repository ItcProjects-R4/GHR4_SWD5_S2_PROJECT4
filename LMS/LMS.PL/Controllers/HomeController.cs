using LMS.BLL.Services.Interfaces;
using LMS.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace LMS.PL.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICourseService _courseService;
        private readonly IEmailSender _emailSender;

        public HomeController(
            ICourseService courseService,
            IEmailSender emailSender)
        {
            _courseService = courseService;
            _emailSender = emailSender;
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
