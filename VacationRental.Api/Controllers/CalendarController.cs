using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Models;
using VacationRental.Services.Interfaces;

namespace VacationRental.Api.Controllers;

[Route("api/v1/calendar")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly IMapper _mapper;

    public CalendarController(ICalendarService calendarService, IMapper mapper)
    {
        _calendarService = calendarService;
        _mapper = mapper;
    }

    [HttpGet]
    public CalendarViewModel Get(int rentalId, DateTime start, int nights)
    {
        if (nights < 0)
            throw new ApplicationException("Nights must be positive");

        var calendarModel = _calendarService.GetCalendarForRental(rentalId, start, nights);

        var calendarViewModel = _mapper.Map<CalendarViewModel>(calendarModel);

        return calendarViewModel;
    }
}