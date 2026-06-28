using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Models
{
    using System;

    namespace LMS.Domain.Models
    {
        public class NewsletterSubscriber
        {
            public int Id { get; set; }
            public required string Email { get; set; }
            public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
        }
    }

}
