using AutoMapper;
using Newtonsoft.Json;
using ReLifeAssessment.Application.Payment.Models;
using ReLifeAssessment.Repositories.Payment.Entities;

namespace ReLifeAssessment.Repositories.Payment.Profiles;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<CompanyPaymentAccountEntity, CompanyPaymentAccount>().ReverseMap();

        CreateMap<CompanyPaymentEntity, CompanyPayment>()
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<PaymentMethodsDetails>(src.Details)));

        CreateMap<CompanyPayment, CompanyPaymentEntity>()
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Details)));
    }
}