using Common.Helpers;

namespace GenshinImpact.Api.Features.Banners.Command;

public sealed record CreateBannerCommand(string Link) : IRequest
{
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
