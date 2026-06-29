namespace LMS.Domain.ViewModels.Student
{
    public class CheckoutResponseViewModel
    {
        public bool IsFree { get; set; }
        public string CourseTitle { get; set; }
        public int CourseId { get; set; }
        public string PaymobRedirectUrl { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}