namespace Spending.Infrastructure.Entities;

public class BaseTrackedEntity : IPersistentEntity, ITrackedEntity
{
    public string CreatedBy { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
