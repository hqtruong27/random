namespace Hoyoverse.Mapper;

public class ProfileMapper
{
    private static readonly Lazy<IMapper> _lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod!.IsPublic || p.GetMethod.IsAssembly;
            cfg.AddProfile<OrganizationProfile>();
        });
        var mapper = config.CreateMapper();
        return mapper;
    });

    public static IMapper Mapper => _lazy.Value;
}


