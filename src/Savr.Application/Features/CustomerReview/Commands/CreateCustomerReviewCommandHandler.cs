using FluentResults;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Savr.Application.Abstractions.Messaging;
using Savr.Domain.Entities;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;


namespace Savr.Application.Features.CustomerReview.Commands
{
    public record struct CreateCustomerReviewCommand(long ListingId, string UserName, string Comment, int Rating) : ICommand<Result<CustomerReviewDTO>>;

    public class CreateCustomerReviewCommandHandler : ICommandHandler<CreateCustomerReviewCommand, Result<CustomerReviewDTO>>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IListingRepository _listingRepository;
        private readonly ICustomerReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCustomerReviewCommandHandler(
            IHttpContextAccessor contextAccessor,
            IListingRepository listingRepository,
            ICustomerReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _listingRepository = listingRepository;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CustomerReviewDTO>> Handle(CreateCustomerReviewCommand request, CancellationToken cancellationToken)
        {
            var userClaim = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out var userId))
                return Result.Fail("You must be logged in to post a review.");

            var listing = await _listingRepository.GetByIdAsync(request.ListingId, cancellationToken);
            if (listing == null)
                return Result.Fail("Listing not found.");

            var review = new Domain.Entities.CustomerReview(userId, request.UserName, request.Comment, request.Rating);
            listing.AddReview(review);

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<CustomerReviewDTO>(review));
        }
    }


    

}

