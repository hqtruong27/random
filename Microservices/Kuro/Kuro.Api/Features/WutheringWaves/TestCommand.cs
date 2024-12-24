using Microsoft.AspNetCore.Mvc;

namespace Kuro.Features.WutheringWaves;

[Get("wuthering-waves/test-command/{name:int}")]
public record TestCommand(string Url, [FromRoute] TestEnum Name) : ICommand<int>;

public class TestCommandHandler() : CommandHandler<TestCommand, int>
{
    public async override Task<int> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(0);
    }
}

public enum TestEnum
{
    Test1,
    Test2
}