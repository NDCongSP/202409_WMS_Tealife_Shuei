namespace SchedulerTask.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string CronExpression { get; set; }
        public string LocalPath { get; set; }

        public string AzureFileConnectionString { get; set; }

        public string AzureFileShareName { get; set; }

    }
}