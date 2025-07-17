using AutoMapper;
using MediatR;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;
using Savr.Domain.Abstractions.Persistence.Repositories;
using System.Linq;

namespace Savr.Application.Features.Products.Queries
{
    public class GetListingListQueryHandler : IListQueryHandler<GetListingListQuery, IEnumerable<ListingDTO>>
    {
        private readonly IListingRepository _productRepository;
        private readonly IMapper _mapper;

        public GetListingListQueryHandler(IListingRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListingDTO>> Handle(GetListingListQuery request, CancellationToken cancellationToken)
        {
            var repoResult = await _productRepository.GetListingListAsync(request.pageNumber, request.pageSize,
                request.NameFilter, request.ManufactureEmailFilter, request.PhoneFilter);

            if(repoResult.Any())
            {
                return _mapper.Map<List<ListingDTO>>(repoResult);
            }

            return Enumerable.Empty<ListingDTO>();
        }
    }
}
