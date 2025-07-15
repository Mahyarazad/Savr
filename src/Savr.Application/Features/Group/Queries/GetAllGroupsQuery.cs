using FluentResults;
using Savr.Application.Abstractions.Messaging;


namespace Savr.Application.Features.Group.Queries
{
    public record struct GetAllGroupsQuery : IQuery<Result<IEnumerable<GroupDTO>>>;
}
