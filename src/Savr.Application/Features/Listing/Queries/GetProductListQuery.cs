using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;
namespace Savr.Application.Features.Products.Queries
{
    public record struct GetProductListQuery(int pageNumber, int pageSize,
        string? NameFilter, string? ManufactureEmailFilter, string? PhoneFilter) : IListQuery<IEnumerable<ListingDTO>>
    {
    }
}
