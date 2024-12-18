namespace Infrastructure.Persistence.Schemas;

public class ConveneRecord : AuditableEntity
{
    public string CardPoolType { get; set; } = default!;
    public int Count { get; set; }
    public string Name { get; set; } = default!;
    public string QualityLevel { get; set; } = default!;
    public long ResourceId { get; set; }
    public string ResourceType { get; set; } = default!;
    public DateTime Time { get; set; }
}
