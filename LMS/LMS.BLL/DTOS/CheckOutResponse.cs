namespace LMS.BLL.DTOS
{
    public class CheckoutResponse
    {
        public bool IsFree { get; set; }
        public string CourseTitle { get; set; }
        public string PaymobRedirectUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}