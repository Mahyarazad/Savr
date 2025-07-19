using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Features.Group.Queries;
using Savr.Application.Features.Group;
using Savr.Application.Abstractions.Identity;

namespace Savr.Application.Features.Identity.Queries
{
    public record struct QueryAllUser : IQuery<Result<IEnumerable<ApplicationUserDto>>>;
    public class FetchAllQueryHandler : IQueryHandler<QueryAllUser, Result<IEnumerable<ApplicationUserDto>>>
    {
        private readonly IAuthService _authService;

        public FetchAllQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result<IEnumerable<ApplicationUserDto>>> Handle(QueryAllUser request, CancellationToken cancellationToken)
        {
            return _authService.GetAllUsers();
        }
    }
}
