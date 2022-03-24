global using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyEPOS.Settings
{
    class Logs
    {
        public static void LogError(string ErrorToLog)
        {
            // Check File Exists
            if (!Directory.Exists("SystemLogs")) { Directory.CreateDirectory("SystemLogs"); }
            if (!File.Exists("SystemLogs/SystemLogs.txt")) { File.Create("SystemLogs/SystemLogs.txt"); }

            // Check Content
            if (ErrorToLog != null && ErrorToLog != "")
            {
                // Get Call Method & Time For Logs
                var TimeNow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                StackTrace stackTrace = new StackTrace();
                // Write Log Using Stream
                using (StreamWriter sw = File.AppendText("SystemLogs/SystemLogs.txt"))
                {
                    sw.WriteLine($"🚀 ~ {TimeNow} [Caller: {stackTrace.GetFrame(1).GetMethod().Name}] ~ {ErrorToLog}");
                }
            }
        }
    }
}
