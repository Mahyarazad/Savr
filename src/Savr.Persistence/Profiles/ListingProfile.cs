using AutoMapper;
using Savr.Application.DTOs;
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