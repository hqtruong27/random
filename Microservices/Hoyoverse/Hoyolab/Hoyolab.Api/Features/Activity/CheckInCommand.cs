using Models.Hoyolab;
using MongoDB.Bson;
using System.Text;
using System.Text.Json;

namespace Hoyolab.Api.Features.Activity;

public record CheckInCommand(string DiscordId) : IRequest<List<CheckInResponse>>
{
    public class CheckInCommandHandler(
        ISettingRepository setting,
        IRepository<User, ObjectId> repository,
        ILogger<CheckInCommandHandler> logger) : IRequestHandler<CheckInCommand, List<CheckInResponse>>
    {
        public async Task<List<CheckInResponse>> Handle(CheckInCommand request, CancellationToken cancellationToken)
        {
            var config = await setting.Read<ActivityConfig>("ACTIVITY_CONFIG");
            logger.LogInformation("config: {config}", config);
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

            List<CheckInResponse> result = [];
            foreach (var hoyolab in user.Hoyolabs)
            {
                foreach (var account in hoyolab.Games)
                {
                    switch (account)
                    {
                        case HoyolabGame.GenshinImpact:
                            var gi = await PostAsync(config.Genshin, hoyolab);
                            gi.Name = "GI";
                            result.Add(gi);
                            break;
                        case HoyolabGame.StarRail:
                            var hsr = await PostAsync(config.Hsr, hoyolab);
                            hsr.Name = "HSR";
                            result.Add(hsr);
                            break;
                        case HoyolabGame.HonkaiImpact3:
                            var hi3 = await PostAsync(config.Hi3, hoyolab);
                            hi3.Name = "Hi3";
                            result.Add(hi3);
                            break;
                        case HoyolabGame.ZenlessZoneZero:
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private async Task<CheckInResponse> PostAsync(Config config, HoyolabAccount hoyolab)
        {
            using HttpClient client = new();

            var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
            client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            logger.LogInformation("payload: {url}, {payload}", config.CheckInUrl, payload);
            var response = await client.PostAsync(config.CheckInUrl, content);

            var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

            return result!;
        }
    }
}
