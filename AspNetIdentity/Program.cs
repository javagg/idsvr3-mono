using System;
using Serilog;
using Microsoft.Owin.Hosting;
using WebHost;

namespace AspNetIdentity
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer3 SelfHost";

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            const string url = "http://+:44001";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        }
    }
}
