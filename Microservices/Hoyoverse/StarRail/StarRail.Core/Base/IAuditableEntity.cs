﻿namespace StarRail.Core.Base;

public interface IAuditableEntity<T> : IEntity<T>
{
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
    DateTime? Created { get; set; }
    DateTime? Updated { get; set; }
}