using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace ExclusiveCard.WebAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            NLog.LogManager.Configuration = new NLog.Extensions.Logging.NLogLoggingConfiguration(config.GetSection("NLog"));
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(NLog.LogManager.Configuration).GetCurrentClassLogger();
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");

                //CreateWebHostBuilder(args).Build().Run();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IWebHostEnvironment env = context.HostingEnvironment;

                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
            });


        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseIISIntegration()
        //        .UseStartup<Startup>()
        //        .ConfigureAppConfiguration((context, builder) =>
        //        {
        //            IHostingEnvironment env = context.HostingEnvironment;

        //            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        //        })
        //        .ConfigureLogging(logging =>
        //        {
        //            logging.ClearProviders();
        //            logging.SetMinimumLevel(LogLevel.Trace);
        //        })
        //        .UseNLog();
    }
}
