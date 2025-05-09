using Microsoft.Extensions.DependencyInjection;
using Okala.Domain.Contracts;
using OKala.Infrastructure.Services;
using System.Reflection;
using FluentValidation;
using MediatR;
using OKala.Application.Behaviors;
using OKala.Application.Features.Queries;

namespace OKala.Application.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(x => x.RegisterServicesFromAssembly(assembly));
            services.AddAutoMapper(assembly);

            services.AddScoped<IValidator<GetWeatherInfoQuery>, GetWeatherInfoValidator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
            services.AddScoped<IWeatherService, WeatherService>();

            return services;
        }
    }
}
