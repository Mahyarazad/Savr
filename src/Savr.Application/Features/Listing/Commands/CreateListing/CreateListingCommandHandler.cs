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

namespace Savr.Application.Features.Listings.Commands

{
    public class CreateListingCommandHandler : ICommandHandler<CreateListingCommand, Result<ListingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IListingRepository _listingRepository;
        //private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateListingCommandHandler(IUnitOfWork unitOfWork, IListingRepository ListingRepository, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _listingRepository = ListingRepository;
            
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ListingDTO>> Handle(CreateListingCommand request, CancellationToken cancellationToken)
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

          


            var creationResult = Listing.Create(
                request.Name,
                request.Description,
                request.Location,
                userId,
                request.GroupId,
                request.PreviousPrice,
                request.CurrentPrice,
                request.PriceWithPromotion

            );

            if (creationResult.IsFailed)
            {
                return Result.Fail(creationResult.Errors);
            }

            var entity = creationResult.Value;

            // Convert TagNames (List<string>) to Tag entities
            var tags = request.TagNames
                .Where(tagName => !string.IsNullOrWhiteSpace(tagName))
                .Distinct()
                .Select(tagName => new Tag(tagName.Trim()))
                .ToList();

            await _listingRepository.AddWithTagsAsync(entity, tags, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var dto = _mapper.Map<ListingDTO>(entity);
            return Result.Ok(dto);
        }

    }
}
