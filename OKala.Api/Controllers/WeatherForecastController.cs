using MediatR;
using Microsoft.AspNetCore.Mvc;
using OKala.Application.Dto;
using OKala.Application.Features.Queries;

namespace OKala.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ApiResult<WeatherDataDto>> GetWeatherForecast([FromQuery] GetWeatherInfoQuery query, CancellationToken cancellatinToken = default)
        {
            var result = await _mediator.Send(query, cancellatinToken);
            return Ok(result);
        }
    }
}
