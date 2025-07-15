using FluentResults;
using Savr.Domain.Primitives;


namespace Savr.Domain.Entities;
public class Group : BaseEntity<long>
{
    protected Group() { }

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Listing> _listings = new();
    public IReadOnlyCollection<Listing> Listings => _listings.AsReadOnly();

    private Group(string title, string description, Guid ownerUserId)
    {
        Title = title;
        Description = description;
        OwnerUserId = ownerUserId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Result<Group> Create(string title, string description, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail("Group title is required.");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail("Group description is required.");

        return Result.Ok(new Group(title, description, ownerUserId));
    }

    public Result<Group> AddListing(Listing listing)
    {
        if (listing is null)
            return Result.Fail("Listing cannot be null.");

        _listings.Add(listing);
        return Result.Ok(this);
    }

    public Result<Group> RemoveListing(Listing listing)
    {
        if (!_listings.Contains(listing))
            return Result.Fail("Listing not found in group.");

        _listings.Remove(listing);
        return Result.Ok(this);
    }


    public Result<Group> Update(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail("Title cannot be empty.");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail("Description cannot be empty.");

        Title = title;
        Description = description;

        return Result.Ok(this);
    }

    public Result<Group> Activate()
    {
        if (IsActive)
            return Result.Fail("Group is already active.");

        IsActive = true;
        return Result.Ok(this);
    }

    public Result<Group> Deactivate()
    {
        if (!IsActive)
            return Result.Fail("Group is already inactive.");

        IsActive = false;
        return Result.Ok(this);
    }
}
