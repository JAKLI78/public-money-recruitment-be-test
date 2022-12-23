using VacationRental.Services.Interfaces;

namespace VacationRental.Services;

public class DateProvider : IDateProvider
{
    public DateTime GetCurrentDate()
    {
        return DateTime.UtcNow.Date;
    }
}