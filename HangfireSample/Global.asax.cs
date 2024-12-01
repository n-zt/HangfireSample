using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Amazon.Util.Internal;
using Hangfire;

namespace HangfireSample
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HangfireConnection"].ConnectionString;

            //Configuring Hangfire to use SQL Server
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString);  // Setting the database connection for Hangfire

            // Starting the Hangfire Server
            var options = new BackgroundJobServerOptions
            {
                WorkerCount = 1 // Defining the number of workers that will run concurrently
            };

            // BackgroundJobServer is starting
            new BackgroundJobServer(options);

            RouteTable.Routes.MapPageRoute("HangfireDashboard", "hangfire", "~/hangfire.aspx");
            
            RecurringJob.AddOrUpdate(
            "DailyNotificationJob",
             () => ExpiryNotification.CheckAndSendNotificationsStatic(connectionString), // Calling the static method directly here
             "* * * * *"
             );
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}