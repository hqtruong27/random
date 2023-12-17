using Hoyolab.Services.Interfaces;
using Hoyoverse.Infrastructure.Entities;
using Hoyoverse.Infrastructure.Repositories;
using Models.Hoyolab;
using MongoDB.Bson;
using System.Text;
using System.Text.Json;

namespace Hoyolab.Services.Services;

public class ActivityService(IRepository<User, ObjectId> repository, ISettingRepository setting) : IActivityService
{
    private readonly ISettingRepository _setting = setting;
    private readonly IRepository<User, ObjectId> _repository = repository;

    public async Task<CheckInResponse> CheckInAsync(CheckInRequest request)
    {
        var setting = await _setting.Read<ActivityConfig>("ACTIVITY_CONFIG");

        var user = await _repository.FirstOrDefaultAsync(x => x.Discord.Id == request.DiscordId);
        if (user == null)
        {
            return new CheckInResponse
            {
                Code = -1,
                Message = "Login Discord first"
            };
        }

        using HttpClient client = new();
        var payload = JsonSerializer.Serialize(new CheckInRequest { ActId = setting.Act.Genshin });

        client.DefaultRequestHeaders.Add("Cookie", user.Hoyolab.Cookie);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(setting.CheckInUrl, content);

        var responseJson = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(responseJson);

        return result!;
    }
}
