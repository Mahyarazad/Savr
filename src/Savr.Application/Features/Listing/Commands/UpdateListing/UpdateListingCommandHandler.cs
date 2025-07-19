using AutoMapper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;
using Savr.Application.Helpers;
using Savr.Domain.Entities;
namespace Savr.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateListingCommandHandler : ICommandHandler<UpdateListingCommand, Result>
    {
        private readonly IListingRepository _listingRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateListingCommandHandler(IListingRepository productRepository, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _listingRepository = productRepository; 
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateListingCommand request, CancellationToken cancellationToken)
        {


            var userClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "Uid");

            var emailClaim = _contextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (userClaim is null || emailClaim is null || !Guid.TryParse(userClaim.Value, out Guid userId))
            {
                return Result.Fail("You must be logged in to update the product.");
            }

            var exists = await _listingRepository.AnyAsync(request.Id, cancellationToken);
            if (!exists)
            {
                return Result.Fail($"Product with ID {request.Id} does not exist.");
            }

            var ownsProduct = await _listingRepository.DoesUserOwnThisListingAsync(request.Id, userId, cancellationToken);
            if (!ownsProduct)
            {
                return Result.Fail($"You do not own the product with ID {request.Id}.");
            }

            var product = await _listingRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                return Result.Fail($"Could not load product with ID {request.Id}.");
            }



            var updateResult = product.Update(request.Name, request.Description, request.Location, 
                request.PreviousPrice, request.CurrentPrice, request.PriceWithPromotion ,request.GroupId,userId);
            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }

            if (request.TagNames is { Count: > 0 })
            {
                var tags = request.TagNames
                    .Where(tagName => !string.IsNullOrWhiteSpace(tagName))
                    .Distinct()
                    .Select(tagName => new Tag(tagName.Trim()))
                    .ToList();

                _listingRepository.UpdateWithTags(product, tags);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }

            await _listingRepository.UpdateAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return Result.Ok();
        }

    }
}
