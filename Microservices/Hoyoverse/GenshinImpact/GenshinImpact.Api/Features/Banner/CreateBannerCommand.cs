using Common.Helpers;
using Dispatcher;

namespace GenshinImpact.Api.Features.Banner;

public class CreateBannerCommand : IRequest
{
    public required string Link { get; set; }

    public class CreateBannerCommandHandler(IRepository<BannerInfo, string> repository) : IRequestHandler<CreateBannerCommand>
    {
        public Task Handle(CreateBannerCommand request, CancellationToken cancellationToken)
        {
            var query = UrlQueryHelper.Populate<UrlQuery>(request.Link);

            return repository.InsertAsync(new BannerInfo
            {
                Id = query.GachaId,
                Region = query.Region,
                GameVersion = query.GameVersion,
            });
        }
    }
}
