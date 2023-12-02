using System.Text.Json;

namespace WebApi.Services;

public interface ISpendingService
{

}

public class SpendingService(ILogger<SpendingService> logger
    , StatisticsDbContext context
    , IMapper mapper) : Protos.Spending.SpendingBase, ISpendingService
{
    private readonly ILogger _logger = logger;
    private readonly StatisticsDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public override async Task<GetSpendingResponse> Get(GetRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Start: get spending {Id}", request.Id);

        var spending = await _context.Spendings.FirstOrDefaultAsync(x => x.Id == request.Id);
        return _mapper.Map<GetSpendingResponse>(spending);
    }

    public override async Task<GetSpendingsByUserIdResponse> GetSpendingsByUserId(GetSpendingsByUserIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Start: get spending by userId {request}", JsonSerializer.Serialize(request));

        var spendings = await _context.Spendings.Where(x => x.UserId == request.UserId).OrderByDescending(x => x.Created).ToListAsync();

        return new() { Items = { spendings.Select(_mapper.Map<GetSpendingResponse>) } };
    }

    public override async Task<Response> Create(CreateSpendingRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Start: create spending {request}", JsonSerializer.Serialize(request));

        await _context.Spendings.AddAsync(_mapper.Map<Spending>(request));

        await _context.SaveChangesAsync();

        return new Response
        {
            Code = StatusCodes.Status200OK,
            Description = "create spending success"
        };
    }
}

