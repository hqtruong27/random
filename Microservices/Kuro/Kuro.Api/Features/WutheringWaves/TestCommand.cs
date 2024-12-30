namespace Kuro.Features.WutheringWaves;

[Post("wuthering-waves/test-command")]
public record TestCommand(string Url, TestEnum Name) : ICommand<int>;

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