using AutoMapper;
using MediatR;
using Savr.Application.Abstractions;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;
using Savr.Application.Features.Listing;

using System.Linq;

namespace Savr.Application.Features.Products.Queries
{
    public class GetListingListQueryHandler : IListQueryHandler<GetListingListQuery, PagedResult<ListingDTO>>
    {
        private readonly IListingRepository _productRepository;
        private readonly IMapper _mapper;

        public GetListingListQueryHandler(IListingRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ListingDTO>> Handle(GetListingListQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetListingListAsync(request.pageNumber, request.pageSize,
               request.Filters);

            
        }
    }
}
