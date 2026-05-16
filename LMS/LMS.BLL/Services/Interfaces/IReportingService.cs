using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.BLL.Services.Interfaces
{
    public interface IReportingService
    {
        Task<IEnumerable<Payment>> GetFinancialReportsAsync();
    }
}
