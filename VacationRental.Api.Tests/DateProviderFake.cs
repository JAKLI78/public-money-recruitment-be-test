using System;
using VacationRental.Services.Interfaces;

namespace VacationRental.Api.Tests;

public class DateProviderFake : IDateProvider
{
    public DateTime GetCurrentDate()
    {
        return new DateTime(2001, 01, 01);
    }
}