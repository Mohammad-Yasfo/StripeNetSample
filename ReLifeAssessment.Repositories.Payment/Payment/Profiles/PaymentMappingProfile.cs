using AutoMapper;
using Newtonsoft.Json;
using StripeNetSample.Application.Payment.Models;
using StripeNetSample.Repositories.Payment.Entities;

namespace StripeNetSample.Repositories.Payment.Profiles;

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