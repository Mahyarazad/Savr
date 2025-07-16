using AutoMapper;
using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Domain.Abstractions.Persistence.Repositories;

namespace Savr.Application.Features.Group.Queries
{
    public class GetAllGroupsQueryHandler : IQueryHandler<GetAllGroupsQuery, Result<IEnumerable<GroupDTO>>>
    {

        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GetAllGroupsQueryHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<GroupDTO>>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var result =  await _groupRepository.GetAllAsync(cancellationToken);

            if (result.Any())
            {
                return Result.Ok( _mapper.Map<IEnumerable<GroupDTO>>(result));
            }

            return Result.Ok(Enumerable.Empty<GroupDTO>());
        }
    }
}
