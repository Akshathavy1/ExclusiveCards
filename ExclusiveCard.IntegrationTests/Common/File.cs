using ExclusiveCard.Data.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.Common
{
    public class File
    {
        public static async Task<List<dto.Files>> CreateFiles(int partnerId)
        {
            var status = await ServiceHelper.Instance.StatusService.GetAll();
            var reqs = new List<dto.Files>
            {
                new dto.Files
                {
                    Name = "ER-SUBS-2019-08-10.txt",
                    PartnerId = partnerId,
                    Type = "PartnerTrans",
                    StatusId = status.FirstOrDefault(x => x.Name == Status.Created && x.Type == StatusType.FileStatus).Id,
                    PaymentStatusId = status.FirstOrDefault(x => x.Name == Status.Unpaid && x.Type == StatusType.FilePayment).Id,
                    TotalAmount = 12.48m,
                    CreatedDate = DateTime.UtcNow.AddDays(-12),
                    ChangedDate = DateTime.UtcNow.AddDays(-12),
                    PaidDate = null,
                    UpdatedBy = null
                },
                new dto.Files
                {
                    Name = "ER-SUBS-2019-08-11.txt",
                    PartnerId = partnerId,
                    Type = "PartnerTrans",
                    StatusId = status.FirstOrDefault(x => x.Name == Status.Created && x.Type == StatusType.FileStatus).Id,
                    PaymentStatusId = status.FirstOrDefault(x => x.Name == Status.Unpaid && x.Type == StatusType.FilePayment).Id,
                    TotalAmount = 10.08m,
                    CreatedDate = DateTime.UtcNow.AddDays(-11),
                    ChangedDate = DateTime.UtcNow.AddDays(-11),
                    PaidDate = null,
                    UpdatedBy = null
                },
                new dto.Files
                {
                    Name = "ER-SUBS-2019-08-12.txt",
                    PartnerId = partnerId,
                    Type = "PartnerTrans",
                    StatusId = status.FirstOrDefault(x => x.Name == Status.Created && x.Type == StatusType.FileStatus).Id,
                    PaymentStatusId = status.FirstOrDefault(x => x.Name == Status.Paid && x.Type == StatusType.FilePayment).Id,
                    TotalAmount = 14.75m,
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    ChangedDate = DateTime.UtcNow.AddDays(-10),
                    PaidDate = null,
                    UpdatedBy = null
                },
                new dto.Files
                {
                    Name = "ER-SUBS-2019-08-13.txt",
                    PartnerId = partnerId,
                    Type = "PartnerTrans",
                    StatusId = status.FirstOrDefault(x => x.Name == Status.Created && x.Type == StatusType.FileStatus).Id,
                    PaymentStatusId = status.FirstOrDefault(x => x.Name == Status.Paid && x.Type == StatusType.FilePayment).Id,
                    TotalAmount = 10.54m,
                    CreatedDate = DateTime.UtcNow.AddDays(-9),
                    ChangedDate = DateTime.UtcNow.AddDays(-9),
                    PaidDate = null,
                    UpdatedBy = null
                }
            };

            var response = new List<dto.Files>();
            foreach (var req in reqs)
            {
                response.Add(await ServiceHelper.Instance.PartnerTransactionService.AddAsync(req));
            }

            return response;
        }
    }
}
