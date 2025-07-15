

using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Domain.Abstractions.Persistence.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using System.Net;

namespace Savr.Application.Features.Group.Commands
{
    internal class DeleteGroupCommandHandler : ICommandHandler<DeleteGroupCommand, Result>
    {
        private readonly IGroupRepository _groupRepository;
        //private readonly IValidator<DeleteGroupCommand> _validator;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGroupCommandHandler(
            IGroupRepository groupRepository,
            //IValidator<DeleteGroupCommand> validator,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            //_validator = validator;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            //var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            //if (!validationResult.IsValid)
            //{
            //    return Result.Fail(HttpStatusCode.BadRequest.ToString());
            //}

            if (!await _groupRepository.AnyAsync(request.Id, cancellationToken))
            {
                return Result.Fail(HttpStatusCode.NotFound.ToString());
            }

            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail(HttpStatusCode.Unauthorized.ToString());
            }

            var ownsGroup = await _groupRepository.DoesUserOwnGroupAsync(request.Id, userId, cancellationToken);
            if (!ownsGroup)
            {
                return Result.Fail(HttpStatusCode.Forbidden.ToString());
            }

            await _groupRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
