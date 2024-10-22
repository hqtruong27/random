namespace StarRail.Core.Base;

public class Entity<T> : IAuditableEntity<T>
{
    public virtual T Id { get; protected set; } = default!;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }

    public bool IsTransient()
    {
        return Id != null && Id.Equals(default(T));
    }
}