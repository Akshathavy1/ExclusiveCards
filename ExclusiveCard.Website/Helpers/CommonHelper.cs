using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using NLog;

namespace ExclusiveCard.Website.Helpers
{
    public class CommonHelper
    {
        public const string THUMBNAIL_SUFFIX = "__1";
        public const string MEDIUM_SUFFIX = "__2";
        public const string LARGE_SUFFIX = "__3";
        public const string FEATURE_SUFFIX = "__4";

        public static IConfiguration AppSettings { get; set; }
        public static int PageSize { get; set; }
        public static int BranchPageSize { get; set; }
        public static int HomePageSize { get; set; }
        public static int TransactionCount { get; set; }
        public static int MaxCashbackAmount { get; set; }

        public static string ContainerName { get; set; }
        public static string ImageCategory { get; set; }
        public static string BlobConnectionString { get; set; }

        public static string NoReplyAddress { get; set; }

        public static string SendGridAPIKey { get; set; }

        
        public static Logger Logger { get; set; }

        public static void Initialize()
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            PageSize = Convert.ToInt32(AppSettings["PageSize"]);
            int home = Convert.ToInt32(AppSettings["HomePageSize"]);
            if (home <= 100)
            {
                HomePageSize = home;
            }
            else if (home > 100)
            {
                HomePageSize = 100;
            }

            ContainerName = AppSettings["ContainerName"].ToString();
            ImageCategory = AppSettings["ImageCategory"].ToString();
            BlobConnectionString = AppSettings["BlobConnectionString"].ToString();
            TransactionCount = Convert.ToInt32(AppSettings["TransactionCount"]);
            MaxCashbackAmount = Convert.ToInt32(AppSettings["MaxCashbackAmount"]);
            NoReplyAddress = AppSettings["NoReplyEmailAddress"];
            SendGridAPIKey = AppSettings["NoReplyEmailAddress"];
            BranchPageSize = Convert.ToInt32(AppSettings["BranchPageSize"]);
            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
