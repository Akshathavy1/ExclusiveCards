using ExclusiveCard.Data.Context;
using ExclusiveCard.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class DbSeeding
    {
        public static IConfiguration AppSettings { get; set; }

        public static async Task Initialize(ExclusiveContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seeding of User Roles
            await CreateRoles(roleManager, Roles.RootUser.ToString(), "Root User");
            await CreateRoles(roleManager, Roles.AdminUser.ToString(), "Admin User");
            await CreateRoles(roleManager, Roles.BackOfficeUser.ToString(), "Back Office User");
            await CreateRoles(roleManager, Roles.User.ToString(), "User");
            await CreateRoles(roleManager, Roles.PartnerAPI.ToString(), "User");

            AppSettings = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            //create blob Storage if not exists
            //CreateBlobStorage(AppSettings["BlobConnectionString"].ToString());
            //Seed root user
            //Look for any user
            #region Create user

            IdentityUser user = await userManager.FindByNameAsync("exclusiverootuser");
            if (user == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "rootuser@exclusive.com", UserName = "exclusiverootuser" };
                await CreateUser(iUser, userManager, Roles.RootUser.ToString());
            }

            IdentityUser adminUser = await userManager.FindByNameAsync("exclusiveadminuser");
            if (adminUser == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "adminuser@exclusive.com", UserName = "exclusiveadminuser" };
                await CreateUser(iUser, userManager, Roles.AdminUser.ToString());
            }

            IdentityUser adminUser1 = await userManager.FindByNameAsync("Adam@exclusivecard.co.uk");
            if (adminUser1 == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "Adam@exclusivecard.co.uk", UserName = "Adam@exclusivecard.co.uk" };
                await CreateUsersExclusive(iUser, userManager, Roles.AdminUser.ToString());
            }

            IdentityUser adminUser2 = await userManager.FindByNameAsync("Zoe@exclusivecard.co.uk");
            if (adminUser2 == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "Zoe@exclusivecard.co.uk", UserName = "Zoe@exclusivecard.co.uk" };
                await CreateUsersExclusive(iUser, userManager, Roles.AdminUser.ToString());
            }

            IdentityUser adminUser3 = await userManager.FindByNameAsync("Neil@exclusivecard.co.uk");
            if (adminUser3 == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "Neil@exclusivecard.co.uk", UserName = "Neil@exclusivecard.co.uk" };
                await CreateUsersExclusive(iUser, userManager, Roles.AdminUser.ToString());
            }

            IdentityUser adminUser4 = await userManager.FindByNameAsync("Chris@exclusivecard.co.uk");
            if (adminUser4 == null)
            {
                IdentityUser iUser = new IdentityUser { Email = "Chris@exclusivecard.co.uk", UserName = "Chris@exclusivecard.co.uk" };
                await CreateUsersExclusive(iUser, userManager, Roles.AdminUser.ToString());
            }

            #endregion

            //#region Social Media Companies

            ////Seeding of Social Media Companies
            //if (!context.SocialMediaCompany.Any())
            //{
            //    List<SocialMediaCompany> socialMedia = new List<SocialMediaCompany>
            //    {
            //        new SocialMediaCompany{Name = "Facebook", IsEnabled = true },
            //        new SocialMediaCompany{Name = "Twitter", IsEnabled = true },
            //        new SocialMediaCompany{Name = "Instagram", IsEnabled = true },
            //        new SocialMediaCompany{Name = "Pinterest", IsEnabled = true}
            //    };

            //    foreach (SocialMediaCompany company in socialMedia)
            //    {
            //        context.SocialMediaCompany.Add(company);
            //    }
            //    context.SaveChanges();
            //}

            //#endregion

            //#region Status

            ////Seed Status
            //List<Data.Models.Status> statuses = new List<Data.Models.Status>
            //{
            //    new Data.Models.Status {Name = "Active", Type = "Offer", IsActive = true},
            //    new Data.Models.Status {Name = "Inactive", Type = "Offer", IsActive = true},
            //    new Data.Models.Status {Name = "Needs Review", Type = "Offer", IsActive = true},
            //    new Data.Models.Status {Name = "Disabled", Type = "Offer", IsActive = true},
            //    new Data.Models.Status {Name = "To Be Deleted", Type = "Offer", IsActive = true},
            //    new Data.Models.Status {Name = "Deleted", Type = "Offer", IsActive = true},

            //    new Data.Models.Status {Name = "New", Type = "Import", IsActive = true},
            //    new Data.Models.Status {Name = "Processing", Type = "Import", IsActive = true},
            //    new Data.Models.Status {Name = "Migrated", Type = "Import", IsActive = true},
            //    new Data.Models.Status {Name = "Complete", Type = "Import", IsActive = true},

            //    new Data.Models.Status {Name = "Pending", Type = "Cashback", IsActive = true},
            //    new Data.Models.Status {Name = "Confirmed", Type = "Cashback", IsActive = true},
            //    new Data.Models.Status {Name = "Received", Type = "Cashback", IsActive = true},
            //    new Data.Models.Status {Name = "Due", Type = "Cashback", IsActive = true},
            //    new Data.Models.Status {Name = "PaidOut", Type = "Cashback", IsActive = true},
            //    new Data.Models.Status {Name = "Declined", Type = "Cashback", IsActive = true},

            //    new Data.Models.Status {Name = "Pending", Type = "Gift", IsActive = true},
            //    new Data.Models.Status {Name = "Confirmed", Type = "Gift", IsActive = true},
            //    new Data.Models.Status {Name = "Complete", Type = "Gift", IsActive = true},

            //    new Data.Models.Status {Name = "Pending", Type = "MembershipCard", IsActive = true},
            //    new Data.Models.Status {Name = "Active", Type = "MembershipCard", IsActive = true},
            //    new Data.Models.Status {Name = "Expired", Type = "MembershipCard", IsActive = true},

            //    new Data.Models.Status {Name = "New", Type = "StagingCashbackTransactions", IsActive = true},
            //    new Data.Models.Status {Name = "In Progress", Type = "StagingCashbackTransactions", IsActive = true},
            //    new Data.Models.Status {Name = "Completed", Type = "StagingCashbackTransactions", IsActive = true},
            //    new Data.Models.Status {Name = "Failed", Type = "StagingCashbackTransactions", IsActive = true},

            //    new Data.Models.Status {Name = "Uploading", Type = "StagingTransactionFiles", IsActive = true},
            //    new Data.Models.Status {Name = "Uploaded", Type = "StagingTransactionFiles", IsActive = true},
            //    new Data.Models.Status {Name = "Processing", Type = "StagingTransactionFiles", IsActive = true},
            //    new Data.Models.Status {Name = "Processed", Type = "StagingTransactionFiles", IsActive = true},
            //    new Data.Models.Status {Name = "Paused", Type = "StagingTransactionFiles", IsActive = true},
            //    new Data.Models.Status {Name = "Cancelled", Type = "StagingTransactionFiles", IsActive = true},

            //    new Data.Models.Status { Name = "Requested", Type = "PhysicalCardStatus", IsActive = true },
            //    new Data.Models.Status { Name = "Sent", Type = "PhysicalCardStatus", IsActive = true },
            //    new Data.Models.Status { Name = "Temp Card Issued", Type = "PhysicalCardStatus", IsActive = true },

            //    new Data.Models.Status { Name = "Cancelled", Type = "MembershipCard", IsActive = true },

            //    new Data.Models.Status { Name = "Active", Type = "PaypalSubscription", IsActive = true },
            //    new Data.Models.Status { Name = "ActiveProfile", Type = "PaypalSubscription", IsActive = true },
            //    new Data.Models.Status { Name = "PendingProfile", Type = "PaypalSubscription", IsActive = true },
            //    new Data.Models.Status { Name = "Finished", Type = "PaypalSubscription", IsActive = true },
            //    new Data.Models.Status { Name = "Cancelled", Type = "PaypalSubscription", IsActive = true },

            //    new Data.Models.Status { Name = "New", Type = "CustomerCreation", IsActive = true },
            //    new Data.Models.Status { Name = "Processing", Type = "CustomerCreation", IsActive = true },
            //    new Data.Models.Status { Name = "Processed", Type = "CustomerCreation", IsActive = true },
            //};

            //if (!context.Status.Any())
            //{
            //    foreach (Data.Models.Status status in statuses)
            //    {
            //        context.Status.Add(status);
            //    }
            //    context.SaveChanges();
            //}
            //else
            //{
            //    foreach (Data.Models.Status status in statuses)
            //    {
            //        if (!context.Status.Any(x => x.IsActive && x.Name == status.Name && x.Type == status.Type))
            //        {
            //            context.Status.Add(status);
            //        }
            //    }
            //    context.SaveChanges();
            //}

            //#endregion

            //#region Offer Type

            ////offer type
            //List<Data.Models.OfferType> types = new List<Data.Models.OfferType>
            //{
            //    new Data.Models.OfferType {Description = "Cashback", IsActive = true},
            //    new Data.Models.OfferType {Description = "Voucher Code", IsActive = true},
            //    new Data.Models.OfferType {Description = "High Street", IsActive = true},
            //    new Data.Models.OfferType {Description = "Sales", IsActive = true}
            //};

            //if (!context.OfferType.Any())
            //{
            //    foreach (Data.Models.OfferType type in types)
            //    {
            //        context.OfferType.Add(type);
            //    }
            //    context.SaveChanges();
            //}

            ////As per new requirement 
            //Data.Models.OfferType standardType = new Data.Models.OfferType { Description = "Standard Cashback", IsActive = true };
            //if (!context.OfferType.Any(x => x.IsActive && x.Description == standardType.Description))
            //{
            //    context.OfferType.Add(standardType);
            //    context.SaveChanges();
            //}

            //Data.Models.OfferType highType = new Data.Models.OfferType { Description = "Restaurant", IsActive = true };

            //if (!context.OfferType.Any(x => x.IsActive && x.Description == highType.Description))
            //{
            //    context.OfferType.Add(highType);
            //    context.SaveChanges();
            //}

            //#endregion

            //#region Offer List (Public)

            ////Offer List
            //List<Data.Models.OfferList> lists = new List<Data.Models.OfferList>
            //{
            //    new Data.Models.OfferList {ListName = "Best Cashback Offers", Description = "Best Cashback Offers", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 0 },
            //    new Data.Models.OfferList {ListName = "Best Voucher Codes", Description = "Best Voucher Codes", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 0  },
            //    new Data.Models.OfferList {ListName = "Daily Deals", Description = "Daily Deals", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 0  },
            //    new Data.Models.OfferList {ListName = "Ending Soon", Description = "Ending Soon", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 0  },
            //    new Data.Models.OfferList {ListName = "Best High Street Offers", Description = "Best High Street Offers", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 2  },
            //    new Data.Models.OfferList {ListName = "Best Sales Offers", Description = "Best Sales Offers", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 0  },
            //    new Data.Models.OfferList {ListName = "Best Restaurant Offers", Description = "Best Restaurant Offers", IsActive = true, IncludeShowAllLink = true, ShowAllLinkCaption = "Show All", PermissionLevel = 2  }
            //};
            //if (!context.OfferList.Any())
            //{
            //    foreach (Data.Models.OfferList list in lists)
            //    {
            //        context.OfferList.Add(list);
            //    }
            //    context.SaveChanges();
            //}

            //#endregion

            //#region Affiliate & affiliate mappings

            ////Affiliate
            //Data.Models.Affiliate affiliate = new Data.Models.Affiliate
            //{
            //    Name = "AWIN"
            //};
            //if (context.Affiliate.All(x => x.Name != affiliate.Name))
            //{
            //    await context.AddAsync(affiliate);
            //    context.SaveChanges();
            //}

            ////Affiliate Mapping Rule
            //Data.Models.AffiliateMappingRule mappingRule = new Data.Models.AffiliateMappingRule
            //{
            //    AffiliateId = 1,
            //    Description = "Affiliate Membership card alias",
            //    IsActive = true
            //};

            //if (!context.AffiliateMappingRule.Any(x => x.IsActive && x.Description == mappingRule.Description
            //    && x.AffiliateId == mappingRule.AffiliateId))
            //{
            //    context.AffiliateMappingRule.Add(mappingRule);
            //    context.SaveChanges();
            //}
            //mappingRule = context.AffiliateMappingRule.FirstOrDefault(x => x.IsActive && x.Description == mappingRule.Description
            //                                                  && x.AffiliateId == mappingRule.AffiliateId);
            //Data.Models.AffiliateMapping affiliateMapping = new Data.Models.AffiliateMapping
            //{
            //    //AffiliateId = 1,
            //    AffilateValue = "ClickRef",
            //    AffiliateMappingRuleId = mappingRule.Id,
            //    //AffiliateFileId = null,
            //    ExclusiveValue = null
            //};

            //if (!context.AffiliateMapping.Any(x =>
            //    x.AffilateValue == affiliateMapping.AffilateValue &&
            //    x.AffiliateMappingRuleId == affiliateMapping.AffiliateMappingRuleId)) //x.AffiliateId == affiliateMapping.AffiliateId &&
            //{
            //    context.AffiliateMapping.Add(affiliateMapping);
            //    context.SaveChanges();
            //}

            //#endregion
            //#region Membersip Plan Type

            ////Membership Plan
            //List<Data.Models.MembershipPlanType> planTypes = new List<Data.Models.MembershipPlanType>
            //{
            //    new Data.Models.MembershipPlanType{ Description = "Personal", IsActive = true }
            //};

            //if (!context.MembershipPlanType.Any())
            //{
            //    await context.AddRangeAsync(planTypes);
            //    context.SaveChanges();
            //}

            //#endregion
            //#region Membership Plans

            ////MemberShipPlan Seeding
            //List<Data.Models.MembershipPlan> membershipPlans = new List<Data.Models.MembershipPlan>
            //{
            //    new Data.Models.MembershipPlan
            //    {
            //        Id = 1,
            //        PartnerId = null,
            //        MembershipPlanTypeId = 1,
            //        NumberOfCards = 0,
            //        Duration = 365,
            //        ValidFrom = DateTime.ParseExact("01/01/2015", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //        ValidTo = DateTime.ParseExact("08/02/2019 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            //        CustomerCardPrice = (decimal)19.95,
            //        CurrencyCode = "GBP",
            //        PartnerCardPrice = 0,
            //        CustomerCashbackPercentage = 100,
            //        PartnerCashbackPercentage = 0,
            //        Description = "Personal v1",
            //        IsActive = true
            //    },

            //    new Data.Models.MembershipPlan
            //    {
            //        Id = 2,
            //        PartnerId = null,
            //        MembershipPlanTypeId = 1,
            //        NumberOfCards = 0,
            //        Duration = 365,
            //        ValidFrom = DateTime.ParseExact("09/02/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //        ValidTo = DateTime.ParseExact("31/12/2019 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            //        CustomerCardPrice = (decimal)19.95,
            //        CurrencyCode = "GBP",
            //        PartnerCardPrice = 0,
            //        CustomerCashbackPercentage = 100,
            //        PartnerCashbackPercentage = 0,
            //        Description = "Personal v2",
            //        IsActive = true
            //    },

            //    new Data.Models.MembershipPlan
            //    {
            //        Id = 3,
            //        PartnerId = null,
            //        MembershipPlanTypeId = 1,
            //        NumberOfCards = 100,
            //        Duration = 365,
            //        ValidFrom = DateTime.ParseExact("09/02/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //        ValidTo = DateTime.ParseExact("31/12/2019 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            //        CustomerCardPrice = 0m,
            //        CurrencyCode = "GBP",
            //        PartnerCardPrice = 0,
            //        CustomerCashbackPercentage = 100,
            //        PartnerCashbackPercentage = 0,
            //        Description = "Personal Complimentary - v1",
            //        IsActive = true,
            //        IsDeleted = false
            //    }
            //};

            //foreach (Data.Models.MembershipPlan plan in membershipPlans)
            //{
            //    if (context.MembershipPlan.Any(x => x.Id == plan.Id))
            //    {
            //        context.MembershipPlan.Update(plan);
            //    }
            //    else
            //    {
            //        plan.Id = 0;
            //        context.MembershipPlan.Add(plan);
            //    }
            //}
            //context.SaveChanges();

            //#endregion

            //#region Payment Provider

            ////PaymentProvider Seeding
            //List<Data.Models.PaymentProvider> paymentProviders = new List<Data.Models.PaymentProvider>
            //{
            //   new Data.Models.PaymentProvider{Name = "Cashback", IsActive = true},
            //   new Data.Models.PaymentProvider{Name = "PayPal", IsActive = true},
            //};

            //if (!context.PaymentProvider.Any())
            //{
            //    foreach (Data.Models.PaymentProvider paymentProvider in paymentProviders)
            //    {
            //        context.PaymentProvider.Add(paymentProvider);
            //    }
            //    context.SaveChanges();
            //}
            //else
            //{
            //    foreach (Data.Models.PaymentProvider paymentProvider in paymentProviders)
            //    {
            //        if (!context.PaymentProvider.Any(x => x.IsActive && x.Name == paymentProvider.Name))
            //        {
            //            context.PaymentProvider.Add(paymentProvider);
            //        }
            //    }
            //    context.SaveChanges();
            //}

            //#endregion
            
            //#region Membership Registration Code

            //MembershipRegistrationCode registrationCode = new MembershipRegistrationCode
            //{
            //    MembershipPlanId = 3,
            //    RegistartionCode = "ExcFree2019",
            //    ValidFrom = DateTime.ParseExact("09/02/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //    ValidTo = DateTime.ParseExact("31/12/2019 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            //    NumberOfCards = 100,
            //    EmailHostName = null,
            //    IsActive = true,
            //    IsDeleted = false
            //};

            //if (!context.MembershipRegistrationCode.Any())
            //{
            //    await context.AddAsync(registrationCode);
            //    context.SaveChanges();
            //}

            //#endregion
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager, string roleName, string normalisedName)
        {
            try
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    IdentityRole iRole = new IdentityRole { Name = roleName, NormalizedName = normalisedName };
                    await roleManager.CreateAsync(iRole);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<bool> CreateBlobStorage(String blobConnectionString)
        {
            bool success = false;
            try
            {
                CloudStorageAccount storageAccount = null;
                string storageConnectionString = blobConnectionString;//ConfigurationManager<>.AppSettings["BlobConnectionString"];

                // Check whether the connection string can be parsed.
                if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                {
                    // If the connection string is valid, proceed with operations against Blob storage here.
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("exclusivecard");
                    bool create = await cloudBlobContainer.CreateIfNotExistsAsync();
                    //_logger.Info("Created container '{0}'", companyId);


                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                    success = true;
                }
                else
                {
                    success = false;
                    // Otherwise, let the user know that they need to define the environment variable.
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        private static async Task CreateUser(IdentityUser user, UserManager<IdentityUser> userManager, string role)
        {

            IdentityResult result = await userManager.CreateAsync(user, "Abcd@1234");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                //User user = new User { Name = "Root", Email = "exclusiveroot" };
                //context.User.Add(user);
                //context.SaveChanges();
            }
        }

        private static async Task CreateUsersExclusive(IdentityUser user, UserManager<IdentityUser> userManager, string role)
        {

            IdentityResult result = await userManager.CreateAsync(user, "Exclusive$$$2019");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                //User user = new User { Name = "Root", Email = "exclusiveroot" };
                //context.User.Add(user);
                //context.SaveChanges();
            }
        }

    }
}
