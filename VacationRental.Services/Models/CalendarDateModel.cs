namespace VacationRental.Services.Models;

public class CalendarDateModel
{
    public DateTime Date { get; set; }
    public List<CalendarBookingModel> Bookings { get; set; }

    public List<CalendarPreparationModel> PreparationTimes { get; set; }
}