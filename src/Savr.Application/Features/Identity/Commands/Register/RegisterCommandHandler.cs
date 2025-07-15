using FluentResults;
using FluentValidation;
using Savr.Application.Abstractions.Identity;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Helpers;

namespace Savr.Application.Features.Identity.Commands.Register
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Result<RegisterCommandResult>>
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterCommand> _validator;
        public RegisterCommandHandler(IAuthService authService, IValidator<RegisterCommand> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        public async Task<Result<RegisterCommandResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);

            if(validationResult.IsValid)
            {
                return await _authService.Register(request, cancellationToken); 
            }

            return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
        }
    }
}
