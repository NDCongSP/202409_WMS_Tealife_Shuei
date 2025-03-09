using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchGenerateReceiptByReceivePlan.Helpers
{
    public static class LogHelpers
    {
        public static void LogFile(string module, string message)
        {
            DateTime today = DateTime.Now;
            string pathBase = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "");
            string dirLog = string.Format("{0}\\AppLogs\\{1}\\{2}", pathBase, today.Year, today.Month);
            string fileName = string.Format("Logs_{0}_{1}.log", module, today.ToString("dd_MM_yy"));
            if (!Directory.Exists(dirLog))
            {
                Directory.CreateDirectory(dirLog);
            }

            var savePath = string.Format("{0}\\{1}", dirLog, fileName);

            if (!string.IsNullOrEmpty(message))
            {
                var messageWrite = $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] " + message;

                File.AppendAllText(savePath, messageWrite + "\n\n");
            }
        }
    }
}
