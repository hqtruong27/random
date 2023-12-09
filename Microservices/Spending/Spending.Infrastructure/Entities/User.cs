namespace Spending.Infrastructure.Entities;

public class User : IdentityUser<string>, ITrackedEntity, IPersistentEntity
{
    public string Surname { get; set; } = default!;
    public string GivenName { get; set; } = default!;
    public string? Address { get; set; }
    public DateTime LastLogin { get; set; }

    [Column("Status")]
    public string StatusString { get => Status.ToString(); private set => Status = Enum.Parse<UserStatus>(value); }

    [Required]
    [NotMapped]
    public UserStatus Status { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime Created { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}