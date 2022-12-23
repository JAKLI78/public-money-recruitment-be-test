using VacationRental.Data.Entities;

namespace VacationRental.Services.Interfaces;

public interface IBookingService
{
    int AddBooking(Booking booking);

    Booking GetBookingById(int id);
}