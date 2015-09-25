using System;
using Serilog;
using Microsoft.Owin.Hosting;
using WebHost;
using System.IO;
namespace AspNetIdentity
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));
            Console.WriteLine(basePath);
            Console.Title = "IdentityServer3 SelfHost";

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            const string url = "http://+:44002/";
            var startOptions = new StartOptions();
            startOptions.Urls.Add(url);
            using (WebApp.Start(url, (appBuilder) =>
            {
                new Startup(basePath).Configuration(appBuilder);
            }))
            {
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        }
    }
}
