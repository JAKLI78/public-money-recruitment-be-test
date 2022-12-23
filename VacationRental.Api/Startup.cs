using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using VacationRental.Core.Middleware;
using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;
using VacationRental.Data.Repositories;
using VacationRental.Services;
using VacationRental.Services.Interfaces;

namespace VacationRental.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program));

        services.AddMvc(options => options.EnableEndpointRouting = false);

        services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacation rental information", Version = "v1" }));

        services.AddSingleton<IDictionary<int, Rental>>(new Dictionary<int, Rental>());
        services.AddSingleton<IDictionary<int, Booking>>(new Dictionary<int, Booking>());
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddTransient<ICalendarService, CalendarService>();
        services.AddTransient<IRentalService, RentalService>();
        services.AddTransient<IBookingService, BookingService>();

        services.AddTransient<IDateProvider, DateProvider>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ErrorHendlerMiddleware>();

        app.UseMvc();
        app.UseSwagger();
        app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
    }
}