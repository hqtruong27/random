using Hoyolab.Services.Interfaces;
using Hoyoverse.Infrastructure.Entities;
using Hoyoverse.Infrastructure.Repositories;
using Models.Hoyolab;
using MongoDB.Bson;
using System.Text;
using System.Text.Json;

namespace Hoyolab.Services.Services;

public class ActivityService(IRepository<User, ObjectId> repository
    , ISettingRepository setting) : IActivityService
{
    private readonly ISettingRepository _setting = setting;

    public async Task AutoCheckInAsync(User user)
    {
        var setting = await _setting.Read<ActivityConfig>("ACTIVITY_CONFIG");

        using HttpClient client = new();

        foreach (var hoyolab in user.Hoyolabs)
        {
            if (!hoyolab.IsAutoCheckIn) continue;
            for (int i = 1; i <= 3; i++)
            {
                switch (i)
                {
                    case 1:
                        await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Genshin);
                        break;
                    case 2:
                        await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Hsr);
                        break;
                    case 3:
                        await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Hi3);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public async Task<List<CheckInResponse>> CheckInAsync(CheckInRequest request)
    {
        var setting = await _setting.Read<ActivityConfig>("ACTIVITY_CONFIG");

        var user = await repository.FirstOrDefaultAsync(x => x.Discord.Id == request.DiscordId);
        if (user == null)
        {
            return
            [
                new CheckInResponse
                {
                    Code = -1,
                    Message = "Login Discord first"
                }
            ];
        }

        using HttpClient client = new();
        var payload = JsonSerializer.Serialize(new CheckInRequest { ActId = setting.Act.Genshin });

        List<CheckInResponse> result = [];
        foreach (var hoyolab in user.Hoyolabs)
        {
            for (int i = 1; i <= 3; i++)
            {
                switch (i)
                {
                    case 1:
                        result.Add(await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Genshin));
                        break;
                    case 2:
                        result.Add(await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Hsr));
                        break;
                    case 3:
                        result.Add(await PostAsync(setting.CheckInUrl, hoyolab, setting.Act.Hi3));
                        break;
                    default:
                        break;
                }
            }
        }

        return result!;
    }

    private static async Task<CheckInResponse> PostAsync(string url, HoyolabAccount hoyolab, string actId)
    {
        using HttpClient client = new();

        var payload = JsonSerializer.Serialize(new { act_id = actId });
        client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

        return result!;
    }
}
