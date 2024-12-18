using Kuro.Features.WutheringWaves.Events;

namespace Kuro.Features.WutheringWaves;

[Post("wuthering-waves/crawl-convene")]
public record CrawlConveneRecordCommand(string Url) : ICommand<int>;

public class CrawlConveneRecordCommandHandler : CommandHandler<CrawlConveneRecordCommand, int>
{
    public async override Task<int> Handle(CrawlConveneRecordCommand request, CancellationToken cancellationToken)
    {
        await Event.PublishAsync(new HelloCreated
        {
            Description = "aaa",
            Name = typeof(HelloCreated).Name
        }, cancellationToken);

        return await Task.FromResult(1);
    }
}