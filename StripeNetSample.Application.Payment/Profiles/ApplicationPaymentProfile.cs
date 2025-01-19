using AutoMapper;
using StripeNetSample.Application.Payment.Dtos;
using StripeNetSample.Application.Payment.Models;

namespace StripeNetSample.Application.Payment.Profiles
{
    public class ApplicationPaymentProfile : Profile
    {
        public ApplicationPaymentProfile()
        {
            CreateMap<PaymentMethodsDetails, PaymentMethodsDetailsDto>()
                .ReverseMap();
        }
    }
}