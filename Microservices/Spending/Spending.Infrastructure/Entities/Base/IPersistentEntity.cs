namespace Data.Entities;

public interface IPersistentEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
