using AutoMapper;
using Savr.Application.Features.CustomerReview;
using Savr.Domain.Entities;

namespace Savr.Persistence.Profiles
{
    public class CustomerReviewProfile : Profile, IProfile
    {
        public CustomerReviewProfile()
        {
            CreateMap<CustomerReview, CustomerReviewDTO>()
                 .ForMember(dest => dest.HelpfulCount, opt => opt.MapFrom(src => src.HelpfulCount));
            
        }
    }   
    
}
