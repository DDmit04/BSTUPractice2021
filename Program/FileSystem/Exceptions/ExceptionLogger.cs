using System;
using System.IO;

namespace Program.FileSystem.Exceptions
{
    class ExceptionLogger
    {
        public static string LogException(Exception exception, string additionalInfo = "")
        {
            var logFolder = DbConfig.LOGS_FILEPATH;
            var logExtension = DbConfig.LOGS_EXTENSION;
            var newLogFilePath = logFolder + "exception_" + GetLogDate() + logExtension;
            newLogFilePath = WriteToLog(newLogFilePath, exception, additionalInfo);
            return newLogFilePath;
        }

        public static string LogWarning(Exception exception, string additionalInfo = "")
        { 
            var logFolder = DbConfig.LOGS_FILEPATH;
            var logExtension = DbConfig.LOGS_EXTENSION;
            var newLogFilePath = logFolder + "warning_" + GetLogDate() + logExtension;
            newLogFilePath = WriteToLog(newLogFilePath, exception, additionalInfo);
            return newLogFilePath;
        }

        private static string WriteToLog(string filepath, Exception exception, string additionalInfo)
        {
            try
            {
                File.Create(filepath).Dispose();
                using (var writer = File.AppendText(filepath))
                {
                    if (additionalInfo != "")
                    {
                        writer.WriteLine("Additional Info: " + additionalInfo);
                    }
                    writer.WriteLine("Date: " + DateTime.Now);
                    writer.WriteLine("Type: " + exception.GetType());
                    writer.WriteLine("Message: " + exception.Message);
                    writer.WriteLine("Stacktrace: \n" + exception.StackTrace);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return "Error while writing log!";
            }
            return filepath;
        }

        private static string GetLogDate()
        {
            return DateTime.Now.ToString("MM_dd_yyyy-HH_mm_ss");
        }

    }
}
