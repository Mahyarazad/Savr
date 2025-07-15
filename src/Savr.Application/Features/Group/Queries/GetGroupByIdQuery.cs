using FluentResults;
using Savr.Application.Abstractions.Messaging;


namespace Savr.Application.Features.Group.Queries
{
    public record struct GetGroupByIdQuery(long Id) : IQuery<Result<GroupDTO>>;
}
