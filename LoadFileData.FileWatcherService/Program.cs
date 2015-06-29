using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using LoadFileData.FileWatcherService.Properties;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.FileWatcherService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //try
            //{
            //    var container = Bootstrapper.Container;
            //    var mainService = container.Resolve<FileWatcherService>();
            //    var servicesToRun = new ServiceBase[] { mainService };
            //    ServiceBase.Run(servicesToRun);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionPolicy.HandleException(ex, PolicyName.Default);
            //}
            //var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser("");
            //parser.TextFieldType = FieldType.Delimited;
            //parser.TrimWhiteSpace

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new FileWatcherService() 
            };
            ServiceBase.Run(ServicesToRun);
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null) return;
            Trace.TraceError(Resources.ExceptionErrorMessage, ex.ToString());
        }

    }

}
