using VacationRental.Data.Entities;

namespace VacationRental.Data.Interfaces;

public interface IBookingRepository : IRepositoryBase<Booking>
{
    void UpdateRange(IEnumerable<Booking> bookings);
}