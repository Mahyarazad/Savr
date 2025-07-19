using AutoMapper;
using Savr.Application.Features.Listing;
using Savr.Domain.Entities;
namespace Savr.Persistence.Profiles
{
    public class ListingProfile : Profile, IProfile
    {
        public ListingProfile()
        {
            CreateMap<Listing, ListingDTO>();
            CreateMap<ListingDTO, Listing>();
        }
    }
}