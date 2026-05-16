using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LMS.BLL.Services.Interfaces;

namespace LMS.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IReportingService _reportingService;

        public AdminController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet]
        public async Task<IActionResult> FinancialReports()
        {
            var reports = await _reportingService.GetFinancialReportsAsync();
            return View("~/Views/Admin/payments.cshtml", reports);
        }
    }
}