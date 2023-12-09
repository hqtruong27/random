namespace Spending.Infrastructure.Entities;

public interface IPersistentEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
