namespace LMS.Domain.ViewModels
{
    public class CheckOutResponseViewModel
    {
        public bool IsFree { get; set; }
        public string CourseTitle { get; set; }
        public string PaymobRedirectUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}