namespace LMS.PL.Helpers
{
    public static class FormattingHelpers
    {
        public static string GetAvatarColor(string? firstName)
        {
            if (string.IsNullOrEmpty(firstName))
                return "var(--accent-color)";

            return firstName.ToUpper()[0] switch
            {
                'M' => "var(--accent-color)",
                'J' => "#0dcaf0",
                'S' => "#198754",
                'D' => "#ffc107",
                'A' => "#dc3545",
                'E' => "#6f42c1",
                _ => "var(--accent-color)"
            };
        }

        public static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";

            if (timeSpan.TotalMinutes < 60)
                return $"Submitted {(int)timeSpan.TotalMinutes} minutes ago";

            if (timeSpan.TotalHours < 24)
                return $"Submitted {(int)timeSpan.TotalHours} hours ago";

            if (timeSpan.TotalDays < 30)
                return $"Submitted {(int)timeSpan.TotalDays} days ago";

            return $"Submitted on {dateTime:MMM dd, yyyy}";
        }
    }
}
