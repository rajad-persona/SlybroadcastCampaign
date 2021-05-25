using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class LogHelper
    {
        private static ILog logger;
        static LogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();
            logger = log4net.LogManager.GetLogger("Persona Batch Job");

        }

        public static void Log(string message)
        {
            logger.Info(message);
        }

        public static void Error(Exception ex, string message = "")
        {
            logger.Error(message + ex.Message, ex);
        }

    }
}
