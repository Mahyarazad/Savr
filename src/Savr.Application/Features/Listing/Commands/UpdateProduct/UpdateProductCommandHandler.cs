using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;
using Savr.Application.Helpers;
using Savr.Domain.Abstractions.Persistence.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
namespace Savr.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Result<ListingDTO>>
    {
        private readonly IListingRepository _productRepository;
        private readonly IValidator<UpdateProductCommand> _validator;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IListingRepository productRepository, IValidator<UpdateProductCommand> validator, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _productRepository = productRepository;
            _validator = validator;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ListingDTO>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result.Fail(ResultErrorParser.GetErrorsFromValidator(validationResult));
            }

            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            var emailClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (userClaim is null || emailClaim is null || !Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail("You must be logged in to update the product.");
            }

            var exists = await _productRepository.AnyAsync(request.Id, cancellationToken);
            if (!exists)
            {
                return Result.Fail($"Product with ID {request.Id} does not exist.");
            }

            var ownsProduct = await _productRepository.DoesUserOwnThisListingAsync(request.Id, userId, cancellationToken);
            if (!ownsProduct)
            {
                return Result.Fail($"You do not own the product with ID {request.Id}.");
            }

            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                return Result.Fail($"Could not load product with ID {request.Id}.");
            }

            var updateResult = product.Update(request.Name, emailClaim.Value, request.ManufacturePhone);
            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }

            await _productRepository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ListingDTO>(product);
            return Result.Ok(dto);
        }

    }
}
