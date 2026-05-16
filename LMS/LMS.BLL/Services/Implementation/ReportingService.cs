using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Repositories.Interfaces;
using LMS.Domain.Models;

namespace LMS.BLL.Services.Implementation
{
    public class ReportingService : IReportingService
    {
        private readonly IPaymentRepository _paymentRepository;

        public ReportingService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<Payment>> GetFinancialReportsAsync()
        {
            return await _paymentRepository.GetAllPaymentsWithDetailsAsync();
        }
    }
}