using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Models;
using VacationRental.Core.Exceptions;
using VacationRental.Data.Entities;
using VacationRental.Services.Interfaces;

namespace VacationRental.Api.Controllers;

[Route("api/v1/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IMapper _mapper;

    public BookingsController(
        IBookingService bookingService,
        IMapper mapper)
    {
        _bookingService = bookingService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("{bookingId:int}")]
    public BookingViewModel Get(int bookingId)
    {
        var booking = _bookingService.GetBookingById(bookingId);

        if (booking is null)
            throw new EntityNotFoundException("Booking not found.");

        return _mapper.Map<BookingViewModel>(booking);
    }

    [HttpPost]
    public ResourceIdViewModel Post(BookingBindingModel model)
    {
        if (model.Nights <= 0)
            throw new ApplicationException("Nigts must be positive.");

        var id = _bookingService.AddBooking(_mapper.Map<Booking>(model));

        var key = new ResourceIdViewModel { Id = id };

        return key;
    }
}