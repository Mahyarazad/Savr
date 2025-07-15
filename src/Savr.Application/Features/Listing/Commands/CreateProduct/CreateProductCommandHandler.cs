using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;
using Savr.Application.Helpers;
using Savr.Domain.Abstractions.Persistence.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;

namespace Savr.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Result<ListingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IListingRepository _productRepository;
        private readonly IValidator<CreateProductCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IListingRepository productRepository, IValidator<CreateProductCommand> validator, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _validator = validator;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ListingDTO>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            var emailClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (userClaim is null || emailClaim is null)
            {
                return Result.Fail("You must be logged in to create a listing.");
            }

            if (!Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail("Invalid user ID format.");
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
            }

            var creationResult = Listing.Create(
                request.Name,
                emailClaim.Value,
                request.ManufacturePhone,
                userId,
                request.GroupId
            );

            if (creationResult.IsFailed)
            {
                return Result.Fail(creationResult.Errors);
            }

            var entity = creationResult.Value;
            await _productRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ListingDTO>(entity);
            return Result.Ok(dto);
        }

    }
}
