using Models.Helpers;
using System.Text.Json;

namespace WebApi.Services;

public class UserService(ILogger<UserService> logger, StatisticsDbContext context) : User.UserBase
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly StatisticsDbContext _context = context;

    public override async Task<Response> Create(CreateUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("START: create user {request}", JsonSerializer.Serialize(request));

        if (await _context.Users.AnyAsync(x => x.Id == request.Id))
        {
            return new Response
            {
                Code = StatusCodes.Status400BadRequest,
                Description = $"Existed user name {request.UserName}"
            };
        }

        var (surname, givenName) = request.GlobalName.ConvertName();

        _context.Users.Add(new Data.Entities.User
        {
            Id = request.Id,
            Email = request.Email,
            UserName = request.UserName,
            Surname = surname,
            GivenName = givenName,
            Status = Common.Enum.UserStatus.Activated,
        });

        await _context.SaveChangesAsync();

        _logger.LogInformation("END: create user success");
        return new Response
        {
            Description = "Success",
            Code = StatusCodes.Status200OK,
        };
    }
}
