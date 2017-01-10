using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.Configuration;
using Nancy.Owin;
using Nancy.TinyIoc;
using Orchard.Environment.Extensions;

namespace Microsoft.AspNetCore.Nancy.Modules
{
    public static class ApplicationBuilderExtensions
    {
        public static ModularApplicationBuilder UseNancyModules(this ModularApplicationBuilder modularApp)
        {
            modularApp.Configure(app =>
            {
                var extensionManager = app.ApplicationServices.GetRequiredService<IExtensionManager>();
                var hostingEnvironment = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Default");

                //// Route the request to the correct tenant specific pipeline
                //app.UseMiddleware<OrchardRouterMiddleware>();

                //// Load controllers
                //var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();

                app.UseOwin(x => x.UseNancy());

                var availableExtensions = extensionManager.GetExtensions();
                using (logger.BeginScope("Loading extensions"))
                {
                    Parallel.ForEach(availableExtensions, new ParallelOptions { MaxDegreeOfParallelism = 4 }, ae =>
                    {
                        try
                        {
                            var extensionEntry = extensionManager
                                .LoadExtensionAsync(ae)
                                .GetAwaiter()
                                .GetResult();

                            if (!extensionEntry.IsError)
                            {
                                
                //                var assemblyPart = new AssemblyPart(extensionEntry.Assembly);
                //                applicationPartManager.ApplicationParts.Add(assemblyPart);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogCritical("Could not load an extension", ae, e);
                        }
                    });
                }

            });

            return modularApp;
        }
    }
}