using Okala.Domain.AggregateRoots;

namespace Okala.Domain.Contracts
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherDataAsync((double Lat, double Lon) cityCoord, CancellationToken cancellationToken = default);
        Task<(double Lat, double Lon)?> GetCordByCityNameAsync(string cityName, CancellationToken cancellationToken = default);
        Task<PollutionData> GetPollutantsAsync((double Lat, double Lon) cityCoord, CancellationToken cancellationToken = default);
    }
}
