namespace Savr.Domain.Primitives
{
    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>
        where TId : notnull
    {
        public TId Id { get; protected set; }

        protected BaseEntity(TId id) => Id = id;

        protected BaseEntity() { } // for EF

        public override bool Equals(object? obj)
        {
            return obj is BaseEntity<TId> entity && Equals(entity);
        }

        public bool Equals(BaseEntity<TId>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
