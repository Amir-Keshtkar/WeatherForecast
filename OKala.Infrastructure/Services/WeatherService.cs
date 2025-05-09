using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Okala.Domain.AggregateRoots;
using Okala.Domain.Contracts;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OKala.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _url;

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration.GetSection("WeatherApiInfo:ApiKey").Value;
            _url = configuration.GetSection("WeatherApiInfo:BaseUrl").Value;
        }

        public async Task<WeatherData> GetWeatherDataAsync((double Lat, double Lon) cityCoord, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("WeatherApi");

            var response = await client.GetFromJsonAsync<WeatherData>(_url + $"/data/2.5/weather?lat={cityCoord.Lat}&lon={cityCoord.Lon}&appid={_apiKey}&units=metric", cancellationToken);
            //var response = await client.GetAsync(_url + $"/data/2.5/weather?lat={cityCoord.Lat}&lon={cityCoord.Lon}&appid={_apiKey}", cancellationToken);
            //if (!response.IsSuccessStatusCode)
            //    return null;

            //var content = await response.Content.ReadAsStringAsync(cancellationToken);
            //var result = JsonConvert.DeserializeObject<WeatherData>(content);
            return response;
        }
        public async Task<(double Lat, double Lon)?> GetCordByCityNameAsync(string cityName, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("WeatherApi");

            var result = await client.GetFromJsonAsync<List<GeoLocation>>(_url + $"/geo/1.0/direct?q={cityName}&limit={1}&appid={_apiKey}", cancellationToken: cancellationToken);
            if (result is null || result.Count == 0)
                return null;
            //var response = await client.GetAsync(_url + $"/geo/1.0/direct?q={cityName}&limit={1}&appid={_apiKey}", cancellationToken: cancellationToken);
            //if (!response.IsSuccessStatusCode)
            //    return null;

            //var content = await response.Content.ReadAsStringAsync(cancellationToken);
            //var result = JsonConvert.DeserializeObject<List<GeoLocation>>(content);
            var city = result.FirstOrDefault();
            return (city.Lat, city.Lon);
        }

        public async Task<PollutionData> GetPollutantsAsync((double Lat, double Lon) cityCoord, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("WeatherApi");

            var response = await client.GetFromJsonAsync<PollutionData>(_url + $"/data/2.5/air_pollution?lat={cityCoord.Lat}&lon={cityCoord.Lon}&appid={_apiKey}&units=metric", cancellationToken);
            //var response = await client.GetAsync(_url + $"/data/2.5/air_pollution?lat={cityCoord.Lat}&lon={cityCoord.Lon}&appid={_apiKey}", cancellationToken);
            //if (!response.IsSuccessStatusCode)
            //    return null;

            //var content = await response.Content.ReadAsStringAsync(cancellationToken);
            //var result = JsonConvert.DeserializeObject<Pollutants>(content);
            return response;
        }
    }

}
