using VacationRental.Data.Entities;

namespace VacationRental.Services.Helpers;

public static class BookingDateHelper
{
    private const int NightsOffset = 1;

    public static DateTime GetBookingEndDate(Booking booking)
    {
        return booking.Start.AddDays(GetRealNightsCount(booking.Nights) + booking.PreparationTimeInDays).Date;
    }

    public static int GetRealNightsCount(int nights)
    {
        return nights - NightsOffset;
    }
}