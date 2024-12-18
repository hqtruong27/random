namespace Wrappers.Services;

public interface IKuroService
{
    [Post("/gacha/record/query")]
    Task ConveneRecordsAsync([Body] ConveneRecordsRequest request);
}
