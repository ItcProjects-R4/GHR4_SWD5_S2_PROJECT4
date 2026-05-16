using LMS.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMS.PL.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View("payment-success");
        }
        public IActionResult PaymentSuccess()
        {
            return View("payment-success");
        }
        public IActionResult Checkout()
        {
        
            return View("checkout");
        }
        public IActionResult purchaseHistory()
        {
            return View("purchase-history");
        }
    }
}
