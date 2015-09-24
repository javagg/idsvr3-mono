using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using Microsoft.Dnx.Runtime;
using System.IO;
using Asp5AspId.IdSvr;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;

namespace Asp5AspId
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json", optional: true)
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IApplicationEnvironment env)
        {
            var certFile = Path.Combine(env.ApplicationBasePath, "idsrv3test.pfx");


            var factory = IdSvr.Factory.Configure();
            var idsrvOptions  = new IdentityServerOptions
            {
                Factory = factory,
                SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                RequireSsl = false
            };

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseIdentityServer(idsrvOptions);
        }
    }
}
