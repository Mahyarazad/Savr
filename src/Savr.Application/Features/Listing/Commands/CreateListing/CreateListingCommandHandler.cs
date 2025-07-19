using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;
using Savr.Application.Features.Identity;
using Savr.Application.Features.Listing;

using Savr.Domain.Entities;

namespace Savr.Application.Features.Listings.Commands

{
    public class CreateListingCommandHandler : ICommandHandler<CreateListingCommand, Result<ListingDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IListingRepository _listingRepository;
        
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


            var creationResult = Savr.Domain.Entities.Listing.Create(
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

            entity.AddTags(request.TagNames);

            await _listingRepository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<ListingDTO>(entity);
            return Result.Ok(dto);
        }


      
    }
}
