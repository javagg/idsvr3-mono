using System;
using IdentityServer3.Core.Configuration;
using IdentityManager.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Owin;
using Microsoft.Framework.DependencyInjection;
using Owin.Security.AesDataProtectorProvider;
using Asp5AspId.IdMgr;
using Asp5AspId.IdSvr;

namespace Microsoft.AspNet.Builder
{
    using DataProtectionProviderDelegate = Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
    using DataProtectionTuple = Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>;

    public static class IApplicationBuilderExtensions
    {
        public static void UseIdentityServer(this IApplicationBuilder app, IdentityServerOptions options)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var builder = new Microsoft.Owin.Builder.AppBuilder();
                    var provider = app.ApplicationServices.GetService<Microsoft.AspNet.DataProtection.IDataProtectionProvider>();

                    builder.Properties["security.DataProtectionProvider"] = new DataProtectionProviderDelegate(purposes =>
                    {
                        var dataProtection = provider.CreateProtector(String.Join(",", purposes));
                        return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                    });
                    builder.UseAesDataProtectorProvider();
                    
                    builder.Map("/admin", adminApp =>
                    {
                        var factory = new IdentityManagerServiceFactory();
                    
                        factory.ConfigureSimpleIdentityManagerService("AspId");
                        //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");
    
                        var adminOptions = new IdentityManagerOptions(){
                            Factory = factory
                        };
                        adminOptions.SecurityConfiguration.RequireSsl = false;
                        adminApp.UseIdentityManager(adminOptions);
                    });

                    options.Factory.ConfigureUserService("AspId");

                    builder.Map("/core", core => {
                        core.UseIdentityServer(options);
                    });
                    var appFunc = builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as Func<IDictionary<string, object>, Task>;
                    return appFunc;
                });
            });
        }
    }
}
