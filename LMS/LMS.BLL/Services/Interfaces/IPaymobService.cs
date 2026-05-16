using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.BLL.Services.Interfaces
{
    public interface IPaymobService
    {
        Task<string> GetPaymentKeyAsync(decimal amount, string studentEmail, string studentFirstName, string studentLastName, string dbPaymentId);
        bool VerifyHmac(JsonElement payload, string receivedHmac);
    }
}