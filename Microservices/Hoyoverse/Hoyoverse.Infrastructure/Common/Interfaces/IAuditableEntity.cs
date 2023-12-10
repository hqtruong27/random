namespace Hoyoverse.Infrastructure.Common.Interfaces;

public interface IAuditableEntity<T> : IEntity<T>
{
    T? CreatedBy { get; set; }
    DateTime? Created { get; set; }
    T? UpdatedBy { get; set; }
    DateTime? Updated { get; set; }
}