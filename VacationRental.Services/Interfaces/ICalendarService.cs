using VacationRental.Services.Models;

namespace VacationRental.Services.Interfaces;

public interface ICalendarService
{
    CalendarModel GetCalendarForRental(int rentalId, DateTime startDate, int countOfNights);
}