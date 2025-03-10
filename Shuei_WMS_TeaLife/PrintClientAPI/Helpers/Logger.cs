﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
namespace SchedulerTask.Helpers
{
    public class Logger : ILog
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public Logger()
        {
        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }
    }
}
