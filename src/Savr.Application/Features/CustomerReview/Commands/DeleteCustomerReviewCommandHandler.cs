using FluentResults;
using Microsoft.AspNetCore.Http;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;


namespace Savr.Application.Features.CustomerReview.Commands
{
    public record struct DeleteCustomerReviewCommand(long ReviewId) : ICommand<Result>;

    public class DeleteCustomerReviewCommandHandler : ICommandHandler<DeleteCustomerReviewCommand, Result>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICustomerReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerReviewCommandHandler(IHttpContextAccessor contextAccessor, ICustomerReviewRepository reviewRepository, IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCustomerReviewCommand request, CancellationToken cancellationToken)
        {
            var userClaim = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Uid");

            if (userClaim is null || !Guid.TryParse(userClaim.Value, out var userId))
                return Result.Fail("You must be logged in to delete a review.");

            var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                return Result.Fail("Review not found.");

            //if (review.UserId != userId)
            //    return Result.Fail("You do not own this review.");

            await _reviewRepository.DeleteAsync(request.ReviewId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

