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
    public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Result<GroupDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRepository _groupRepository;
        //private readonly IValidator<CreateGroupCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateGroupCommandHandler(
            IUnitOfWork unitOfWork,
            IGroupRepository groupRepository,
            //IValidator<CreateGroupCommand> validator,
            IMapper mapper,
            IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _groupRepository = groupRepository;
            //_validator = validator;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<GroupDTO>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null)
            {
                return Result.Fail("You must be logged in to create a group.");
            }

            if (!Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail("Invalid user ID format.");
            }

            //var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            //if (!validationResult.IsValid)
            //{
            //    return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
            //}

            var creationResult = Savr.Domain.Entities.Group.Create(request.Title, request.Description, userId);

            if (creationResult.IsFailed)
            {
                return Result.Fail(creationResult.Errors);
            }

            var entity = creationResult.Value;
            await _groupRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<GroupDTO>(entity);
            return Result.Ok(dto);
        }
    }
}
