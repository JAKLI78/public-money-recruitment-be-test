using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;

namespace VacationRental.Data.Repositories;

public class BookingRepository : RepositoryBase<Booking>, IBookingRepository
{
    public BookingRepository(IDictionary<int, Booking> entities) : base(entities)
    {
    }

    public void UpdateRange(IEnumerable<Booking> bookings)
    {
        foreach (var booking in bookings)
        {
            Update(booking);
        }
    }
}