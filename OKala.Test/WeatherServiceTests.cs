using Moq;
using Microsoft.Extensions.Configuration;
using Okala.Domain.AggregateRoots;
using OKala.Infrastructure.Services;
using System.Net;
using System.Text.Json;
using FluentAssertions;
namespace OKala.Test;
public class WeatherServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConfigurationSection> _apiKeySectionMock;
    private readonly Mock<IConfigurationSection> _baseUrlSectionMock;

    public WeatherServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _configurationMock = new Mock<IConfiguration>();
        _apiKeySectionMock = new Mock<IConfigurationSection>();
        _baseUrlSectionMock = new Mock<IConfigurationSection>();

        _apiKeySectionMock.Setup(x => x.Value).Returns("test-key");
        _baseUrlSectionMock.Setup(x => x.Value).Returns("https://api.openweathermap.org");

        _configurationMock.Setup(x => x.GetSection("WeatherApiInfo:ApiKey")).Returns(_apiKeySectionMock.Object);
        _configurationMock.Setup(x => x.GetSection("WeatherApiInfo:BaseUrl")).Returns(_baseUrlSectionMock.Object);
    }

    [Fact]
    public async Task GetCordByCityNameAsync_WhenValidCity_ReturnsCoordinates()
    {
        // Arrange
        var cityName = "Tehran";
        var geoLocation = new List<GeoLocation>
        {
            new GeoLocation { Lat = 35.6893, Lon = 51.3896, Name = "Tehran", Country = "IR" }
        };
        var responseContent = JsonSerializer.Serialize(geoLocation);
        var httpClient = new HttpClient(new MockHttpMessageHandler(responseContent, HttpStatusCode.OK));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetCordByCityNameAsync(cityName, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Lat.Should().BeApproximately(35.6893, 0.0001);
        result.Value.Lon.Should().BeApproximately(51.3896, 0.0001);
    }

    [Fact]
    public async Task GetCordByCityNameAsync_WhenInvalidCity_ReturnsNull()
    {
        // Arrange
        var cityName = "Shiganshina";
        var responseContent = "[]";
        var httpClient = new HttpClient(new MockHttpMessageHandler(responseContent, HttpStatusCode.OK));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetCordByCityNameAsync(cityName, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task GetWeatherDataAsync_WhenValidCoordinates_ReturnsWeatherData()
    {
        // Arrange
        var coords = (Lat: 35.6893, Lon: 51.3896);
        var weatherData = new WeatherData
        {
            Coord = new Coord { Lat = 35.6893, Lon = 51.3896 },
            Main = new WeatherData.MainData { Temp = 28.84, Humidity = 9, Pressure = 1013 },
            Wind = new Wind { Speed = 2.06, Deg = 180 },
            Weather = new List<Weather> { new Weather { Id = 800, Main = "Clear", Description = "clear sky", Icon = "01d" } },
            Name = "Tehran",
            Cod = 200
        };
        var responseContent = JsonSerializer.Serialize(weatherData);
        var httpClient = new HttpClient(new MockHttpMessageHandler(responseContent, HttpStatusCode.OK));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetWeatherDataAsync(coords, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Main.Temp.Should().BeApproximately(28.84, 0.01);
        result.Main.Humidity.Should().BeInRange(0, 100);
        result.Wind.Speed.Should().BeGreaterThanOrEqualTo(0);
        result.Coord.Lat.Should().BeApproximately(35.6893, 0.0001);
        result.Coord.Lon.Should().BeApproximately(51.3896, 0.0001);
        result.Weather.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetWeatherDataAsync_WhenApiFails_ReturnsNull()
    {
        // Arrange
        var coords = (Lat: 35.6893, Lon: 51.3896);
        var httpClient = new HttpClient(new MockHttpMessageHandler("", HttpStatusCode.InternalServerError));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetWeatherDataAsync(coords, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPollutantsAsync_WhenValidCoordinates_ReturnsPollutionData()
    {
        // Arrange
        var coords = (Lat: 35.6893, Lon: 51.3896);
        var pollutionData = new PollutionData
        {
            Coord = new Coord { Lat = 35.6893, Lon = 51.3896 },
            List = new List<PollutionData.Polluts>
            {
                new PollutionData.Polluts
                {
                    Main = new PollutionData.Polluts.MainData { AQI = 3 },
                    Components = new PollutionData.Polluts.Component { PM2_5 = 25.5, CO = 200.5, NO2 = 10.2 },
                    DT = 1697059200
                }
            }
        };
        var responseContent = JsonSerializer.Serialize(pollutionData);
        var httpClient = new HttpClient(new MockHttpMessageHandler(responseContent, HttpStatusCode.OK));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetPollutantsAsync(coords, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.List.Should().HaveCount(1);
        result.List[0].Main.AQI.Should().BeInRange(0, 200);
        result.List[0].Components.PM2_5.Should().BeInRange(0, 500);
    }

    [Fact]
    public async Task GetPollutantsAsync_WhenApiFails_ReturnsNull()
    {
        // Arrange
        var coords = (Lat: 35.6893, Lon: 51.3896);
        var httpClient = new HttpClient(new MockHttpMessageHandler("", HttpStatusCode.BadRequest));
        _httpClientFactoryMock.Setup(x => x.CreateClient("WeatherApi")).Returns(httpClient);

        var service = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object);

        // Act
        var result = await service.GetPollutantsAsync(coords, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
    {
        _responseContent = responseContent;
        _statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_responseContent)
        });
    }
}