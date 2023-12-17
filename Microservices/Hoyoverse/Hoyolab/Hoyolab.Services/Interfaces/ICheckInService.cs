using Models.Hoyolab;

namespace Hoyolab.Services.Interfaces;

public interface ICheckInService
{
    Task<CheckInResponse> CheckInAsync(CheckInRequest request);
}
