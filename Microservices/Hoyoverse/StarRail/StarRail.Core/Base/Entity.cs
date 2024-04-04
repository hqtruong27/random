namespace StarRail.Core.Base;

public class Entity<T> : IAuditableEntity<T>
{
    private T _id = default!;
    private int? _requestedHashCode;

    public virtual T Id
    {
        get => _id;
        protected set => _id = value;
    }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }

    public bool IsTransient()
    {
        return Id != null && Id.Equals(default(T));
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity<T>)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        Entity<T> item = (Entity<T>)obj;

        if (item.IsTransient() || IsTransient())
            return false;
        else
            return item.Id != null && item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                if (Id != null)
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                else
                    _requestedHashCode = 0;
            }

            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();
    }

    public static bool operator ==(Entity<T> left, Entity<T> right)
    {
        if (Equals(left, null))
            return Equals(right, null);
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity<T> left, Entity<T> right)
    {
        return left != right;
    }
}