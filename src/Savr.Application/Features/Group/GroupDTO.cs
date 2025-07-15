

namespace Savr.Application.Features.Group
{
    public readonly record struct GroupDTO(
        long Id,
        string Title,
        string Description,
        bool IsActive,
        Guid OwnerUserId,
        DateTime CreatedAt
    );
}
