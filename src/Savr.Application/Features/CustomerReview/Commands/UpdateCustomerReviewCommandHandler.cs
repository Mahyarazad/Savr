using FluentResults;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;


namespace Savr.Application.Features.CustomerReview.Commands
{
    public record struct UpdateCustomerReviewCommand(long ReviewId, string Comment, int Rating) : ICommand<Result<CustomerReviewDTO>>;
    public class UpdateCustomerReviewCommandHandler : ICommandHandler<UpdateCustomerReviewCommand, Result<CustomerReviewDTO>>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICustomerReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCustomerReviewCommandHandler(
            IHttpContextAccessor contextAccessor,
            ICustomerReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _contextAccessor = contextAccessor;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CustomerReviewDTO>> Handle(UpdateCustomerReviewCommand request, CancellationToken cancellationToken)
        {
            var userClaim = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out var userId))
                return Result.Fail("You must be logged in to update a review.");

            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review is null)
                return Result.Fail("Review not found.");

            if (review.UserId != userId)
                return Result.Fail("You do not have permission to update this review.");

            // Business rule: Update only the comment and rating
            var updatedReview = new Domain.Entities.CustomerReview(review.UserId, review.UserName, request.Comment, request.Rating);
            review = updatedReview;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<CustomerReviewDTO>(review));
        }
    }

}

