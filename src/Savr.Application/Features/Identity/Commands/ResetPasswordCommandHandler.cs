using FluentResults;
using MediatR;
using Savr.Application.Abstractions.Identity;

namespace Savr.Application.Features.Identity.Commands
{
    public record struct GeneratePasswordResetLinkCommand(string Email, string CallbackUrlBase) : IRequest<Result<string>>;

    public class GeneratePasswordResetLinkHandler : IRequestHandler<GeneratePasswordResetLinkCommand, Result<string>>
    {
        private readonly IAuthService _authService;

        public GeneratePasswordResetLinkHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<string>> Handle(GeneratePasswordResetLinkCommand command, CancellationToken cancellationToken)
        {
            return await _authService.GeneratePasswordResetLink(command);
        }
    }


    public record struct ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;

    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IAuthService _authService;

        public ResetPasswordHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            return await _authService.ResetPassword(command);
        }
    }
}
