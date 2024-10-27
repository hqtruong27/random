namespace GenshinImpact.Core.Base;

public interface IAuditableEntity<T> : IEntity<T>
{
    T? CreatedBy { get; set; }
    T? UpdatedBy { get; set; }
    DateTime? Created { get; set; }
    DateTime? Updated { get; set; }
}

public interface IAuditableEntity : IAuditableEntity<string>;