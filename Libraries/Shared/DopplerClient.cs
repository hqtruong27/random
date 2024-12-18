using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Shared;

public class DopplerClient
{
    private HttpClient _client;
    public DopplerClient(string? dopplerToken = default)
    {
        DopplerToken = dopplerToken;
        _client = GetHttpClientFactory();
    }

    public DopplerClient(string apiVersion, string format, string? dopplerToken = default)
    {
        ApiVersion = apiVersion;
        Format = format;
        DopplerToken = dopplerToken;
        _client = GetHttpClientFactory();
    }

    public string ApiVersion { get; set; } = "v3";
    public string Format { get; set; } = "yaml";
    public string? DopplerToken { get; set; }
    public Uri BaseUri { get; set; } = new Uri("https://api.doppler.com");
    public string Uri => $"{BaseUri}/{ApiVersion}/{ConfigPath}{Format}";
    private string ConfigPath => "configs/config/secrets/download?format=";

    public async Task<string> FetchSecretsAsJsonAsync()
    {
        var jsonResult = await _client.GetStringAsync(Uri);

        var d = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = d.Deserialize<Dictionary<string, string>>(jsonResult);

        return EnvToJsonConverter.ConvertToJson(yaml, EnvToJsonConverter.NamingConvention.CamelCase);
    }

    public async Task<MemoryStream> FetchSecretsAsStreamAsync()
    {
        var jsonResult = await _client.GetStringAsync(Uri);

        var d = new DeserializerBuilder()
             .WithNamingConvention(CamelCaseNamingConvention.Instance)
             .Build();

        var yaml = d.Deserialize<Dictionary<string, string>>(jsonResult);

        return EnvToJsonConverter.ConvertToJsonStream(yaml, EnvToJsonConverter.NamingConvention.CamelCase);
    }

    private HttpClient GetHttpClientFactory()
    {
        _client = new HttpClient();
        var dopplerToken = DopplerToken ?? Environment.GetEnvironmentVariable("DOPPLER_TOKEN");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", dopplerToken);

        return _client;
    }
}
