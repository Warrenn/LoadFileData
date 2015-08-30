using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoadFileData.Web;
using Microsoft.Owin.Hosting;

namespace LoadFileData.Host
{
    class Program
    {
        private static ContextMenu menu;
        private static MenuItem mnuExit;
        private static MenuItem mnuShow;
        private static NotifyIcon notificationIcon;
        private static string url;

        [STAThread]
        private static void Main(string[] args)
        {

            url = ConfigurationManager.AppSettings["host:url"];
            AppDomain.CurrentDomain.SetData("DataDirectory",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
            AppDomain.CurrentDomain.ProcessExit += Application_ApplicationExit;

            var mainTask = Task.Run(() =>
            {
                menu = new ContextMenu();
                mnuExit = new MenuItem("Exit");
                mnuShow = new MenuItem("Show");
                menu.MenuItems.Add(0, mnuShow);
                menu.MenuItems.Add(1, mnuExit);

                notificationIcon = new NotifyIcon
                {
                    Icon = Properties.Resources.LoadFileDataIcon,
                    ContextMenu = menu,
                    Text = "LoadFileData"
                };

                mnuExit.Click += MnuExit_Click;
                mnuShow.Click += MnuShow_Click;

                notificationIcon.Visible = true;
                Application.Run();
            });

            using (WebApp.Start<Startup>(url))
            {
                MnuShow_Click(mnuShow, EventArgs.Empty);
                mainTask.Wait();
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit -= Application_ApplicationExit;
            mnuExit.Click -= MnuExit_Click;
            mnuShow.Click -= MnuShow_Click;
            notificationIcon.Visible = false;
            notificationIcon.Dispose();
            notificationIcon = null;
        }

        private static void MnuShow_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(url);
            }
            catch (Win32Exception)
            {
                Process.Start("IExplore.exe", url);
            }
        }

        private static void MnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
