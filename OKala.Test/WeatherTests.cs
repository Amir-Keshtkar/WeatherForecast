using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using OKala.Application.Features.Queries;
using AutoMapper;
using OKala.Application.Dto;
using Okala.Domain.AggregateRoots;
using Okala.Domain.Contracts;
using static Okala.Domain.AggregateRoots.PollutionData;
using System;
using OKala.Application.Exceptions;

namespace OKala.Test;

public class WeatherTests
{
    [Fact]
    public void GetWeatherInfoValidator_WhenCityNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange 
        var validator = new GetWeatherInfoValidator();
        var query = new GetWeatherInfoQuery { CityName = "" };

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CityName)
              .WithErrorMessage("CityName is required");
    }

    [Fact]
    public async Task GetWeatherInfoValidator_WhenCityNameIsNotExist_ShouldHaveValidationError()
    {
        // Arrange
        var weatherServiceMock = new Mock<IWeatherService>();
        weatherServiceMock.Setup(x => x.GetCordByCityNameAsync("shiganshina", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValueTuple<double, double>?());
        //.ReturnsAsync(Task.FromResult<(double Lat, double Lon)?>(null));

        var mapperMock = new Mock<IMapper>();
        var handler = new GetWeatherInfoQuery.Handler(mapperMock.Object, weatherServiceMock.Object);
        var query = new GetWeatherInfoQuery { CityName = "shiganshina" };

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AppException>()
            .WithMessage("شهر یافت نشد");

        weatherServiceMock.Verify(x => x.GetCordByCityNameAsync("shiganshina", It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetWeatherInfoQueryHandler_WhenValidCity_ReturnsWeatherDataDto()
    {
        // Arrange
        var weatherServiceMock = new Mock<IWeatherService>();
        var mapperMock = new Mock<IMapper>();

        // Mock GetCordByCityNameAsync
        weatherServiceMock.Setup(x => x.GetCordByCityNameAsync("Tehran", It.IsAny<CancellationToken>()))
            .ReturnsAsync((35.6893, 51.3896));

        // Mock GetWeatherDataAsync
        var weatherData = new WeatherData
        {
            Coord = new Coord { Lat = 35.6893, Lon = 51.3896 },
            Main = new WeatherData.MainData { Temp = 301.99, Humidity = 9 },
            Wind = new Wind { Speed = 2.06 },
            Name = "Tehran"
        };
        weatherServiceMock.Setup(x => x.GetWeatherDataAsync(new ValueTuple<double, double>(35.6893, 51.3896), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData);

        // Mock GetPollutantsAsync
        var pollutionData = new PollutionData()
        {
            List = [new Polluts() { Main = new Polluts.MainData() { AQI = 3 }, Components = new Polluts.Component() { PM2_5 = 25.5 } }]
        };
        weatherServiceMock.Setup(x => x.GetPollutantsAsync(new ValueTuple<double, double>(35.6893, 51.3896), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pollutionData);

        // Mock AutoMapper
        var weatherDataDto = new WeatherDataDto
        {
            Temperature = "301.99 °C",
            Humidity = "9 %",
            WindSpeed = 2.06,
            AirQualityIndex = 3,
            Pollutants = new Pollutants() { PM25 = 25.5 },
            Latitude = 35.6893,
            Longitude = 51.3896
        };
        mapperMock.Setup(x => x.Map<WeatherDataDto>(weatherData))
            .Returns(weatherDataDto);
        mapperMock.Setup(x => x.Map(pollutionData, weatherDataDto, typeof(PollutionData), typeof(WeatherDataDto)))
            .Callback<object, object, Type, Type>((src, dest, srcType, destType) => { })
            .Returns(weatherDataDto);

        var handler = new GetWeatherInfoQuery.Handler(mapperMock.Object, weatherServiceMock.Object);
        var query = new GetWeatherInfoQuery { CityName = "Tehran" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Temperature.Should().Be("301.99 °C");
        result.Humidity.Should().Be("9 %");
        result.WindSpeed.Should().BeApproximately(2.06, 0.01);
        result.AirQualityIndex.Should().Be(3);
        result.Pollutants.PM25.Should().BeApproximately(25.5, 0.1);
        result.Latitude.Should().BeApproximately(35.6893, 0.0001);
        result.Longitude.Should().BeApproximately(51.3896, 0.0001);
    }

}
