using AutoMapper;
using ReLifeAssessment.Application.Payment.Dtos;
using ReLifeAssessment.Application.Payment.Models;

namespace ReLifeAssessment.Application.Payment.Profiles
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