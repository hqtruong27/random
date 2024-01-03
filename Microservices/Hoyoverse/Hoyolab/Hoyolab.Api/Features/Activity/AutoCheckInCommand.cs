using Models.Hoyolab;
using System.Text;
using System.Text.Json;

namespace Hoyolab.Api.Features.Activity;

public record AutoCheckInCommand(User User) : IRequest
{
    public class AutoCheckInCommandHandler(ISettingRepository _setting) : IRequestHandler<AutoCheckInCommand>
    {
        public async Task Handle(AutoCheckInCommand request, CancellationToken cancellationToken)
        {
            var setting = await _setting.Read<ActivityConfig>("ACTIVITY_CONFIG");

            using HttpClient client = new();

            foreach (var hoyolab in request.User.Hoyolabs)
            {
                if (!hoyolab.IsAutoCheckIn) continue;
                for (int i = 1; i <= 3; i++)
                {
                    switch (i)
                    {
                        case 1:
                            await PostAsync(setting.Genshin, hoyolab);
                            break;
                        case 2:
                            await PostAsync(setting.Hsr, hoyolab);
                            break;
                        //case 3:
                        //    await PostAsync(setting.Hi3, hoyolab);
                        //    break;
                        default:
                            break;
                    }
                }
            }
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
