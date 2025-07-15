using AutoMapper;
using Savr.Application.Features.Group;
using Savr.Domain.Entities;


namespace Savr.Persistence.Profiles
{
    public class GroupProflie : Profile, IProfile
    {
        public GroupProflie()
        {
            CreateMap<Group, GroupDTO>();
            CreateMap<GroupDTO, Group>();
        }
    }
}
