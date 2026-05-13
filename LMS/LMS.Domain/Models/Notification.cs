using System.ComponentModel.DataAnnotations;


namespace LMS.Domain.Models
{
    public class Notification
    {
        public int Id { get; set; }  

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; }


    }
}