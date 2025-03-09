namespace WebUIFinal
{
    public static class NotificationHelper
    {
        public static void ShowNotification(NotificationService notificationService, NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }
    }
}
