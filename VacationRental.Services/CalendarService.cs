using VacationRental.Core.Exceptions;
using VacationRental.Data.Interfaces;
using VacationRental.Services.Interfaces;
using VacationRental.Services.Models;

namespace VacationRental.Services;

public class CalendarService : ICalendarService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookingRepository _bookingRepository;

    public CalendarService(IRentalRepository rentalRepository, IBookingRepository bookingRepository)
    {
        _rentalRepository = rentalRepository;
        _bookingRepository = bookingRepository;
    }

    public CalendarModel GetCalendarForRental(int rentalId, DateTime startDate, int countOfNights)
    {
        var rental = _rentalRepository.GetById(rentalId);

        if (rental == null)
        {
            throw new EntityNotFoundException("Rental with such id not found.");
        }

        var bookings = _bookingRepository.GetAll();

        var result = new CalendarModel
        {
            RentalId = rentalId,
            Dates = new List<CalendarDateModel>()
        };
        for (var i = 0; i < countOfNights; i++)
        {
            var date = new CalendarDateModel
            {
                Date = startDate.Date.AddDays(i),
                Bookings = new List<CalendarBookingModel>(),
                PreparationTimes = new List<CalendarPreparationModel>()
            };

            var rentalBookings = bookings.Values.Where(b => b.RentalId == rentalId).ToList();

            foreach (var booking in rentalBookings)
            {
                var lastDayDate = booking.Start.AddDays(booking.Nights);

                if (booking.Start <= date.Date
                    && lastDayDate > date.Date)
                {
                    date.Bookings.Add(new CalendarBookingModel { Id = booking.Id, Unit = booking.Unit });
                }
                else
                if (booking.Start <= date.Date
                    && lastDayDate.AddDays(rental.PreparationTimeInDays) > date.Date)
                {
                    date.PreparationTimes.Add(new CalendarPreparationModel { Unit = booking.Unit });
                }
            }

            result.Dates.Add(date);
        }

        return result;
    }
}