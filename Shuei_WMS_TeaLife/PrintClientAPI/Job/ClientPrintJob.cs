using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SchedulerTask.Helpers;
using SchedulerTask.Services;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SchedulerTask.Job
{
    public class ClientPrintJob : CronJobService
    {
        private IServiceScopeFactory _services { get; }
        private ILog _logger;
        private readonly IConfiguration configuration;
        public ClientPrintJob(IConfiguration configuration, IScheduleConfig<ClientPrintJob> config, ILog logger, IServiceScopeFactory services)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _services = services;
            this.configuration = configuration;
            IServiceScope scope = services.CreateScope();
            
            

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("ClientPrintJob starts scanning print .");
            return base.StartAsync(cancellationToken);
        }
        private List<IListFileItem> GetAllFilesFromDirectory(CloudFileDirectory directory)
        {
            List<IListFileItem> results = new List<IListFileItem>();
            FileContinuationToken token = null;
            do
            {
                FileResultSegment resultSegment = directory.ListFilesAndDirectoriesSegmentedAsync(token).GetAwaiter().GetResult();
                
                results.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
                
            }
            while (token != null);
            return results;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                int RetentionPeriodinMinutes;

               // FileDirectory = "C:\\Source\\";
                RetentionPeriodinMinutes = 5;
                DateTime lastMins = DateTime.Now.AddMinutes(-RetentionPeriodinMinutes);
                var files = new DirectoryInfo("Data").GetFiles("*.*");
                foreach (var file in files)
                {

                    if (file.CreationTime < lastMins)
                    {
                        file.Delete();
                    }
                }

            }
            catch(Exception e)
            {
                _logger.Error(e.Message);
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("ClientPrintJob is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
