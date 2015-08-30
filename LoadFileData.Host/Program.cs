using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.Web;
using Microsoft.Owin.Hosting;

namespace LoadFileData.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["host:url"];
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }
}
