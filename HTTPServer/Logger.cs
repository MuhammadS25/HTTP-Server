using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            DateTime now = DateTime.Now;
            sr.WriteLine(now.ToString("F"));
            //message:
            sr.WriteLine(ex.Message);
            // for each exception write its details associated with datetime 
         
        }
    }
}
