namespace Hoyoverse.Persistence.Schemas;

public class RedemptionCode : AuditableEntity
{
    public required string Code { get; init; }
    public required string Type { get; init; }
    public RedemptionCode()
    {
        
    }

    public RedemptionCode(string code, string type)
    {
        Code = code;
        Type = type;
    }
}
