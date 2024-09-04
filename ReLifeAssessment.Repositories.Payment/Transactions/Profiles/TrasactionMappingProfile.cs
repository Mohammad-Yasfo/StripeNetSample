using AutoMapper;
using ReLifeAssessment.Repositories.Payment.Transaction.Entities;

namespace ReLifeAssessment.Repositories.Payment.Transaction.Profiles
{
    public class TrasactionMappingProfile : Profile
    {
        public TrasactionMappingProfile()
        {
            CreateMap<Application.Payment.Models.Transaction, TransactionEntity>().ReverseMap();
        }
    }
}