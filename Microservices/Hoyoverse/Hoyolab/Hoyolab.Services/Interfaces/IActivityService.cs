using Models.Hoyolab;

namespace Hoyolab.Services.Interfaces;

public interface IActivityService
{
    Task<CheckInResponse> CheckInAsync(CheckInRequest request);
}
