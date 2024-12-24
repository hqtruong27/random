namespace Kuro.Features.WutheringWaves;

[Get("wuthering-waves/crawl-convene")]
public record CrawlConveneRecordCommand(string Url, string Name, string Description) : ICommand<ConveneRecord>
{
    public int Position { get; set; }
    public int? La { get; set; }
    public TestEnum Last { get; set; }
}

public class CrawlConveneRecordCommandHandler : CommandHandler<CrawlConveneRecordCommand, ConveneRecord>
{
    public async override Task<ConveneRecord> Handle(CrawlConveneRecordCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new ConveneRecord());
    }
}