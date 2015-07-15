using System;
using System.Threading.Tasks;
using LoadFileData.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LoadFileData.Web.Startup))]
namespace LoadFileData.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //loop through all the json files 
            //create the settings for the handlers
            //create the sttings for the readers
            //create the filehandler settings
            //create the folder monitors
            //create the types from json
            //intialize the database with the types
            //create the filehandlers

        }
    }
}
