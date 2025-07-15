using FluentResults;
using FluentValidation;
using Savr.Application.Abstractions.Identity;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Helpers;

namespace Savr.Application.Features.Identity.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginCommandResult>>
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginCommand> _validator;

        public LoginCommandHandler(IAuthService authService, IValidator<LoginCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task<Result<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if(validationResult.IsValid)
            {
                return await _authService.Login(request, cancellationToken);    
            }

            return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
        }
    }
}
