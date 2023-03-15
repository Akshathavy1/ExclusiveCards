using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ILogger = NLog.ILogger;

namespace Exclusive.Jobs
{
    class Program
    {
        static void Main(string[] args)
        {

                ILogger log = LogManager.GetCurrentClassLogger();

#if DEBUG
                log.Error("Exclusive.Jobs processing started");
#endif

                if (args.Length == 0)
                {
                    log.Info("Argument Null Exception");
                    return;
                }
                else if (args[0].Contains("?") || args[0].ToLower() == "h" || args[0].ToLower().Contains("help"))
                {
                    log.Info("Valid Arguments:");
                    log.Info(string.Join(", ", Enum.GetNames(typeof(JobName))));
                    return;
                }

                log.Info("Exclusive.Jobs processing started");

                Task.Run(async () =>
                {
                    await processing(args);
                }).Wait();

                log.Info("All Jobs Complete.");
        }

        static async Task<int> processing(string[] args)
        {
            ILogger log = LogManager.GetCurrentClassLogger();
            int result = -1;

            JobName jobToRun = JobName.None;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (Enum.TryParse(args[i], true, out jobToRun) && jobToRun != JobName.None)
                    {
                        log.Info($"{jobToRun} Started");

                        IServiceCollection services = new ServiceCollection();
                        // Startup.cs configuration
                        Startup startup = new Startup();
                        startup.ConfigureServices(services);
                        IServiceProvider serviceProvider = services.BuildServiceProvider();

                        // Get Service and call method
                        var marketingService = serviceProvider.GetService<IMarketingService>();
                        log.Info($"Job Name: {jobToRun}");

                        switch (jobToRun)
                        {
                            case JobName.ManageMarketingContacts:
                                log.Info(" ManageMarketingContacts");
                                result = await marketingService.ManageMarketingContacts();
                                break;
                            case JobName.ManageMarketingEvents:
                                log.Info(" ManageMarketingEvents");
                                result = await marketingService.ManageMarketingEvents();
                                break;
                            case JobName.All:

                                log.Info(" ManageMarketingContacts >");
                                result = await marketingService.ManageMarketingContacts();

                                log.Info(" ManageMarketingEvents >");
                                result += await marketingService.ManageMarketingEvents();
                                break;
                            default:
                                log.Info($"Job action : {jobToRun} is not defined");
                                break;
                        }

                        log.Info($"Job Complete, {result} actions performed");

                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }
    }
}