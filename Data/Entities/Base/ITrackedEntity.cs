namespace Data.Entities;

public interface ITrackedEntity
{
    string CreatedBy { get; set; }
    DateTime Created { get; set; }
    string? UpdatedBy { get; set; }
    DateTime? Updated { get; set; }
}