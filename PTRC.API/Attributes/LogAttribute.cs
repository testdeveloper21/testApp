using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PTRC.API.Attributes
{
    public class ErrorLogAttribute : ActionFilterAttribute, IExceptionFilter
    {      

        /// <summary>
        /// 
        /// </summary>  
        /// <param name="context"></param>
        public void OnException(ExceptionContext filterContext)
        {
            LogWriter _logManager = new LogWriter();
            _logManager.LogWrite(string.Format("Exception Occoured on {0}", (string)filterContext.RouteData.Values["action"]));
            _logManager.LogWrite(string.Format("Exception is {0}", filterContext.Exception.StackTrace));
        }
    }
    public class LogWriter
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        public void LogWrite(string logMessage)
        {
            string filename = String.Format("{1}_{0}", @"log.txt", DateTime.Now.ToString("yyyy-MM-dd"));
            string fullpath = "filePathfromConfig" + filename;
            try
            {
                if (File.Exists(fullpath))
                {
                    using (StreamWriter sw = File.AppendText(fullpath))
                    {
                        //This is where you will write your text to the new file if the other one was in use
                        Log(logMessage, sw);

                        sw.Flush();
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(fullpath))
                    {
                        //This is where you will write your text to the new file if the other one was in use
                        Log(logMessage, sw);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch
            {
                using (StreamWriter sw = File.CreateText(fullpath))
                {
                    //This is where you will write your text to the new file if the other one was in use
                    Log(logMessage, sw);
                    sw.Flush();
                    sw.Close();
                }
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="txtWriter"></param>
        public void Log(string logMessage, TextWriter txtWriter)
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("  :");
            txtWriter.WriteLine("  :{0}", logMessage);
            txtWriter.WriteLine("-------------------------------");
        }
    }
}