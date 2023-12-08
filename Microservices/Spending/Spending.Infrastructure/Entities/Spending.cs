namespace Data.Entities;

public class Spending : BaseTrackedEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "text")]
    public string Description { get; set; } = default!;
    [Required]
    [NotMapped]
    public SpendingStatus Status { get; set; }
    [Column("Status")]
    public string StatusString { get => Status.ToString(); private set => Status = Enum.Parse<SpendingStatus>(value); }
    public string UserId { get; set; } = default!;
    public SpendingPurpose Purpose { get; set; } = default!;
}
