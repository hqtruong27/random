using Core;
using Models.Hoyolab;

namespace Hoyolab.Api.Features;

public class TestHandler : IRequestHandler<CheckInRequest, string>
{
    public Task<string> Handle(CheckInRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Test");
    }
}
