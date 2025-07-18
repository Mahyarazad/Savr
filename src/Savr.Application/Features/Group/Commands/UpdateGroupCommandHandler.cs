

using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Helpers;
using Savr.Domain.Abstractions.Persistence.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;


namespace Savr.Application.Features.Group.Commands
{
    public class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand, Result<GroupDTO>>
    {
        private readonly IGroupRepository _groupRepository;
        //private readonly IValidator<UpdateGroupCommand> _validator;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateGroupCommandHandler(
            IGroupRepository groupRepository,
            //IValidator<UpdateGroupCommand> validator,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _groupRepository = groupRepository;
            //_validator = validator;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GroupDTO>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            //var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            //if (!validationResult.IsValid)
            //{
            //    return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
            //}

            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail("You must be logged in to update the group.");
            }

            var exists = await _groupRepository.AnyAsync(request.Id, cancellationToken);
            if (!exists)
            {
                return Result.Fail($"Group with ID {request.Id} does not exist.");
            }

            var ownsGroup = await _groupRepository.DoesUserOwnGroupAsync(request.Id, userId, cancellationToken);
            if (!ownsGroup)
            {
                return Result.Fail($"You do not own the group with ID {request.Id}.");
            }

            var group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken);

            if (group is null)
            {
                return Result.Fail($"Could not load group with ID {request.Id}.");
            }

            var updateResult = group.Update(request.Title, request.Description, request.IsActive);


            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }

            await _groupRepository.UpdateAsync(group, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<GroupDTO>(group);
            return Result.Ok(dto);
        }
    }
}
