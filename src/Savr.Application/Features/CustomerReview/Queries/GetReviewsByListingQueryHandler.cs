using AutoMapper;
using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Repositories;


namespace Savr.Application.Features.CustomerReview.Queries
{
    public record struct GetReviewsByListingQuery(long ListingId) : IQuery<Result<IEnumerable<CustomerReviewDTO>>>;

    public class GetReviewsByListingQueryHandler : IQueryHandler<GetReviewsByListingQuery, Result<IEnumerable<CustomerReviewDTO>>>
    {
        private readonly ICustomerReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public GetReviewsByListingQueryHandler(ICustomerReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CustomerReviewDTO>>> Handle(GetReviewsByListingQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _reviewRepository.GetAllAsync(cancellationToken);

            if (reviews == null)
                return Result.Ok(Enumerable.Empty<CustomerReviewDTO>());

            return Result.Ok(_mapper.Map<IEnumerable<CustomerReviewDTO>>(reviews));
        }
    }
}
