using AutoMapper;
using StripeNetSample.Repositories.Payment.Transaction.Entities;

namespace StripeNetSample.Repositories.Payment.Transaction.Profiles
{
    public class TrasactionMappingProfile : Profile
    {
        public TrasactionMappingProfile()
        {
            CreateMap<Application.Payment.Models.Transaction, TransactionEntity>().ReverseMap();
        }
    }
}