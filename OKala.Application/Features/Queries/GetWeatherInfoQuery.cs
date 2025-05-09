using MediatR;
using OKala.Application.Dto;
using Okala.Domain.Contracts;
using FluentValidation;
using AutoMapper;
using Microsoft.Extensions.Options;
using OKala.Application.Exceptions;
using Okala.Domain.AggregateRoots;

namespace OKala.Application.Features.Queries
{
    public class GetWeatherInfoQuery : IRequest<WeatherDataDto>
    {
        public string CityName { get; set; }

        public class Handler : IRequestHandler<GetWeatherInfoQuery, WeatherDataDto>
        {
            private readonly IMapper _mapper;
            private readonly IWeatherService _weatherService;

            public Handler(IMapper mapper, IWeatherService weatherService)
            {
                _mapper = mapper;
                _weatherService = weatherService;
            }

            public async Task<WeatherDataDto> Handle(GetWeatherInfoQuery request, CancellationToken cancellationToken)
            {
                var coords = await _weatherService.GetCordByCityNameAsync(request.CityName, cancellationToken)
                    ?? throw new AppException("شهر یافت نشد");

                var weatherInfo = await _weatherService.GetWeatherDataAsync(coords, cancellationToken);
                var pollutants = await _weatherService.GetPollutantsAsync(coords, cancellationToken);

                var result = _mapper.Map<WeatherDataDto>(weatherInfo);
                _mapper.Map(pollutants, result, typeof(PollutionData), typeof(WeatherDataDto));
                return result;
            }
        }
    }
    #region Validator
    public class GetWeatherInfoValidator : AbstractValidator<GetWeatherInfoQuery>
    {
        public GetWeatherInfoValidator()
        {
            RuleFor(x => x.CityName)
                .NotEmpty().WithMessage("CityName is required");
        }

    }
    #endregion
}
