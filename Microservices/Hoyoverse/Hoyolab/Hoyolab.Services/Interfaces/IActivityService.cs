using Hoyoverse.Infrastructure.Entities;
using Models.Hoyolab;

namespace Hoyolab.Services.Interfaces;

public interface IActivityService
{
    Task<List<CheckInResponse>> CheckInAsync(CheckInRequest request);
    Task AutoCheckInAsync(User user);
}
