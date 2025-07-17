using FluentResults;
using Savr.Application.DTOs;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;

namespace Savr.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<Result<LoginCommandResult>> Login(LoginCommand command, CancellationToken cancellationToken = default);

        Task<Result<RegisterCommandResult>> Register(RegisterCommand command, CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<ApplicationUserDto>>> GetAllUsers(CancellationToken cancellationToken = default);
    }
}
