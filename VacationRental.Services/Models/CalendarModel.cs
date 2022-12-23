namespace VacationRental.Services.Models;

public class CalendarModel
{
    public int RentalId { get; set; }
    public List<CalendarDateModel> Dates { get; set; }
}