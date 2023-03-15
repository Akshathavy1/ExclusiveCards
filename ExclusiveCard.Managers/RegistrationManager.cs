using AutoMapper;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Providers;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using db = ExclusiveCard.Data.Models;

using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public class RegistrationManager : IRegistrationManager
    {
        #region Private Fields and Constructor

        private readonly IRepository<db.RegistrationCodeSummary> _registrationCodeSummaryRepo;
        private readonly IRepository<db.MembershipRegistrationCode> _membershipRegistrationCodeRepo;
        private readonly IAzureStorageProvider _azureStorageProvider;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public RegistrationManager(IRepository<db.RegistrationCodeSummary> registrationCodeSummaryRepo,
                                   IRepository<db.MembershipRegistrationCode> membershipRegistrationCodeRepo,
                                   IAzureStorageProvider azureStorageProvider,
                                   IMapper mapper)
        {
            _registrationCodeSummaryRepo = registrationCodeSummaryRepo;
            _membershipRegistrationCodeRepo = membershipRegistrationCodeRepo;
            _azureStorageProvider = azureStorageProvider;
            _logger = LogManager.GetLogger(GetType().FullName);
            _mapper = mapper;
        }

        #endregion Private Fields and Constructor

        /// <see cref="IRegistrationManager.CreateRegistrationBatchAsync(dto.RegistrationCodeSummary)"/>
        public async Task<bool> CreateRegistrationBatchAsync(dto.RegistrationCodeSummary summary)
        {
            try
            {
                _logger.Info("CreateRegistrationBatchAsync");
                // calling a stored procedure to generate registration codes and saving the generated data to
                var storagePath = $"RegistrationCodes_{summary.MembershipPlanId}_";
                DbCommand cmd = _membershipRegistrationCodeRepo.LoadStoredProcedure("[Exclusive].[SP_GenerateMembershipCodeAndStore]");
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@membershipPlanId", summary.MembershipPlanId);
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@numberofCodes", summary.NumberOfCodes);
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@numberofUses", summary.NumberOfUses);
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@validFrom", summary.ValidFrom);
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@validTo", summary.ValidTo);
                cmd = _membershipRegistrationCodeRepo.WithSqlParam(cmd, "@storagePath", storagePath);
                var membershipCodes = _membershipRegistrationCodeRepo.ExecuteStoredProcedure<db.MembershipRegistrationCode>(cmd);
                if (membershipCodes != null)
                {
                    if (membershipCodes.Count > 1)
                    {
                        storagePath = storagePath + membershipCodes.FirstOrDefault().RegistrationCodeSummaryId + ".csv";
                        IEnumerable<string> codes = new string[] { new string(@"""Registration_Codes""") };
                        codes = codes.Concat(from m in membershipCodes select @$"""{m.RegistartionCode}""");

                        var stream = new ByteStream(Encode(codes, Encoding.UTF8));
                        await _azureStorageProvider.SaveStreamAsync(summary.BlobConnectionString, summary.ContainerName, storagePath, stream);

                        return true;
                    }
                    else if (membershipCodes.Count == 1)
                    {
                        summary.StoragePath = string.Empty;
                        return true;
                    }
                }
                else
                {
                    throw new ArgumentNullException("Registration codes SP did not return any results");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return false;
        }

        /// <see cref="IRegistrationManager.DownloadRegistrationsAsync(dto.RegistrationCodeSummary)"/>
        public async Task<byte[]> DownloadRegistrationsAsync(dto.RegistrationCodeSummary summary)
        {
            var stream = await _azureStorageProvider.GetStreamAsync(summary.BlobConnectionString,
                        summary.ContainerName, summary.StoragePath);
            return stream;
        }

        /// <see cref="IRegistrationManager.GetAllSummaries(int)"/>
        public List<dto.RegistrationCodeSummary> GetAllSummaries(int membershipPlanId)
        {
            var dbRegistrationCodeSummaryUnique = _registrationCodeSummaryRepo.FilterNoTrack(x => x.MembershipPlanId == membershipPlanId && x.NumberOfCodes > 1 && x.StoragePath != null).ToList();
            var dbRegistrationCodeSummaryShared = _registrationCodeSummaryRepo.Include(x => x.MembershipRegistrationCodes).Where(x => x.MembershipPlanId == membershipPlanId && x.StoragePath == null).ToList();
            if (dbRegistrationCodeSummaryUnique != null && dbRegistrationCodeSummaryUnique.Any())
            {
                dbRegistrationCodeSummaryShared.AddRange(dbRegistrationCodeSummaryUnique);
            }

            var dtoRegistrationCodeSummary = _mapper.Map<List<dto.RegistrationCodeSummary>>(dbRegistrationCodeSummaryShared);
            return dtoRegistrationCodeSummary;
        }

        ///<see cref="IRegistrationManager.GetAllRegistrationsAsync(int)"/>
        public async Task<List<dto.MembershipRegistrationCode>> GetAllRegistrationsAsync(int registrationCodeSummaryId)
        {
            var dbMembershipRegistrationCode = await _membershipRegistrationCodeRepo.FilterNoTrackAsync(x => (x.RegistrationCodeSummaryId == registrationCodeSummaryId)
                                        && x.IsActive && !x.IsDeleted);
            var dtoMembershipRegistrationCode = _mapper.Map<List<dto.MembershipRegistrationCode>>(dbMembershipRegistrationCode);
            return dtoMembershipRegistrationCode;
        }

        public static IEnumerable<byte> Encode(IEnumerable<string> input, Encoding encoding)
        {
            byte[] newLine = encoding.GetBytes(Environment.NewLine);
            foreach (string line in input)
            {
                byte[] bytes = encoding.GetBytes(line);
                foreach (byte b in bytes)
                    yield return b;
                foreach (byte b in newLine)
                    yield return b;
            }
        }
    }
}