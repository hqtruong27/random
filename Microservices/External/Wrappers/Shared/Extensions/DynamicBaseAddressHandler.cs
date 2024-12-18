namespace Wrappers.Shared.Extensions;

public class DynamicBaseAddressHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var headerName = nameof(Hoyoverse.Act);
        var act = request.Headers.FirstOrDefault(x => x.Key == headerName).Value.FirstOrDefault();
        var relativeUri = request.RequestUri?.PathAndQuery;
        switch (act)
        {
            case nameof(Hoyoverse.Act.GenshinImpact):
                request.RequestUri = new($"{configuration["Hoyoverse:GenshinImpact:BaseUrl"]}{relativeUri}");
                break;
            case nameof(Hoyoverse.Act.Hsr):
                request.RequestUri = new($"{configuration["Hoyoverse:Hsr:BaseUrl"]}{relativeUri}");
                break;
            default:
                break;
        }

        request.Headers.Remove(headerName);

        return base.SendAsync(request, cancellationToken);
    }
}

