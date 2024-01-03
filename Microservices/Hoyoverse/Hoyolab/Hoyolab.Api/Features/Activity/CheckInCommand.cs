using Models.Hoyolab;
using MongoDB.Bson;
using System.Text;
using System.Text.Json;

namespace Hoyolab.Api.Features.Activity;

public record CheckInCommand(string DiscordId) : IRequest<List<CheckInResponse>>
{
    public class CheckInCommandHandler(ISettingRepository _setting, IRepository<User, ObjectId> repository) : IRequestHandler<CheckInCommand, List<CheckInResponse>>
    {
        public async Task<List<CheckInResponse>> Handle(CheckInCommand request, CancellationToken cancellationToken)
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

            List<CheckInResponse> result = [];
            foreach (var hoyolab in user.Hoyolabs)
            {
                for (int i = 1; i <= 3; i++)
                {
                    switch (i)
                    {
                        case 1:
                            result.Add(await PostAsync(setting.Genshin, hoyolab));
                            break;
                        case 2:
                            result.Add(await PostAsync(setting.Hsr, hoyolab));
                            break;
                        case 3:
                            result.Add(await PostAsync(setting.Hi3, hoyolab));
                            break;
                        default:
                            break;
                    }
                }
            }

            return result!;
        }

        private static async Task<CheckInResponse> PostAsync(Config config, HoyolabAccount hoyolab)
        {
            using HttpClient client = new();

            var payload = JsonSerializer.Serialize(new { act_id = config.ActId });
            client.DefaultRequestHeaders.Add("Cookie", hoyolab.Cookie);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(config.CheckInUrl, content);

            var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<CheckInResponse>(stream);

            return result!;
        }
    }
}
