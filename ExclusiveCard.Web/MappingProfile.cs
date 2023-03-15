using AutoMapper;
using ExclusiveCard.WebAdmin.ViewModels;
using System.Globalization;
using System;

namespace ExclusiveCard.WebAdmin
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Services.Models.DTOs.ContactDetail, ContactsViewModel>();
            CreateMap<ContactsViewModel, Services.Models.DTOs.ContactDetail>();
            CreateMap<Data.Models.SSOConfiguration, Services.Models.DTOs.SSOConfiguration>();

            CreateMap<Services.Models.DTOs.MembershipPlan, WhiteLabelSettingMembershipPlan>()
                .ForMember(x => x.ValidFrom, x => x.MapFrom(y => y.ValidFrom != null ? Convert.ToDateTime(y.ValidFrom, new CultureInfo("en-GB")).ToString("dd-MM-yyyy") : null))
                .ForMember(x => x.ValidTo, x => x.MapFrom(y => y.ValidTo != null ? Convert.ToDateTime(y.ValidTo, new CultureInfo("en-GB")).ToString("dd-MM-yyyy") : null));

            CreateMap<WhiteLabelSettingMembershipPlan, Services.Models.DTOs.MembershipPlan>()
                .ForMember(x => x.ValidFrom, x => x.MapFrom(y => !string.IsNullOrEmpty(y.ValidFrom) ? Convert.ToDateTime(y.ValidFrom, new CultureInfo("en-GB")).ToString("yyyy-MM-dd") : null))
                .ForMember(x => x.ValidTo, x => x.MapFrom(y => !string.IsNullOrEmpty(y.ValidTo) ? Convert.ToDateTime(y.ValidTo, new CultureInfo("en-GB")).ToString("yyyy-MM-dd") : null));

            CreateMap<Services.Models.DTOs.AgentCode, WhiteLabelSettingsAgents>().ReverseMap();
            CreateMap<Services.Models.DTOs.RegistrationCodeSummary, RegistrationCodesSummary>()
                .ForMember(x => x.ListOfRegistrationCodes, x => x.MapFrom(y => y.MembershipRegistrationCodes))
                .ForMember(x => x.ValidFrom, x => x.MapFrom(y => y.ValidFrom != null ? Convert.ToDateTime(y.ValidFrom, new CultureInfo("en-GB")).ToString("dd-MM-yyyy") : null))
                .ForMember(x => x.ValidTo, x => x.MapFrom(y => y.ValidTo != null ? Convert.ToDateTime(y.ValidTo, new CultureInfo("en-GB")).ToString("dd-MM-yyyy") : null));
            
            CreateMap<RegistrationCodesSummary, Services.Models.DTOs.RegistrationCodeSummary>()
                .ForMember(x => x.MembershipRegistrationCodes, x => x.MapFrom(y => y.ListOfRegistrationCodes))
                .ForMember(x => x.ValidFrom, x => x.MapFrom(y => !string.IsNullOrEmpty(y.ValidFrom) ? Convert.ToDateTime(y.ValidFrom, new CultureInfo("en-GB")).ToString("yyyy-MM-dd") : null))
                .ForMember(x => x.ValidTo, x => x.MapFrom(y => !string.IsNullOrEmpty(y.ValidTo) ? Convert.ToDateTime(y.ValidTo, new CultureInfo("en-GB")).ToString("yyyy-MM-dd") : null));
        }
    }
}