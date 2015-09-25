using IdentityServer3.Core.Configuration;
using Owin;
using SelfHost.Config;
using Owin.Security.AesDataProtectorProvider;

namespace SelfHost
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseAesDataProtectorProvider();
            var factory = new IdentityServerServiceFactory();
            factory
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get())
                .UseInMemoryUsers(Users.Get());

            var options = new IdentityServerOptions
            {
                SiteName = "IdentityServer3 (self host)",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                RequireSsl = false
            };
            appBuilder.UseIdentityServer(options);
        }
    }
}