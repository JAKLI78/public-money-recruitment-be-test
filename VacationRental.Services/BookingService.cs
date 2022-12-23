using VacationRental.Core.Exceptions;
using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;
using VacationRental.Services.Interfaces;
using static VacationRental.Services.Helpers.BookingDateHelper;

namespace VacationRental.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRentalRepository _rentalRepository;

    public BookingService(IBookingRepository bookingRepository, IRentalRepository rentalRepository)
    {
        _bookingRepository = bookingRepository;
        _rentalRepository = rentalRepository;
    }

    public int AddBooking(Booking booking)
    {
        var rental = _rentalRepository.GetById(booking.RentalId);

        if (rental is null)
            throw new EntityNotFoundException("Rental not found");

        var bookings = _bookingRepository.GetAll().Values.Where(b => b.RentalId == booking.RentalId);

        var bookedUnits = new List<int>();

        for (var i = 0; i < booking.Nights; i++)
        {
            var count = 0;
            foreach (var existingBooking in bookings)
            {
                var existingBookingEndDate = GetBookingEndDate(existingBooking);
                var newBookingEndDate = GetBookingEndDate(booking);

                if (existingBooking.Start <= booking.Start.Date && existingBookingEndDate > booking.Start.Date
                || (existingBooking.Start < newBookingEndDate && existingBookingEndDate >= newBookingEndDate)
                || (existingBooking.Start > booking.Start && existingBookingEndDate < newBookingEndDate))
                {
                    count++;
                    bookedUnits.Add(existingBooking.Unit);
                }
            }
            if (count >= rental.Units)
                throw new ApplicationException("Not available");
        }

        var freeUnitNumber = GetFreeUnitNumber(bookedUnits, rental.Units);

        if (freeUnitNumber == 0)
        {
            throw new ApplicationException("Not available");
        }

        booking.Unit = freeUnitNumber;

        return _bookingRepository.Add(booking);
    }

    public Booking GetBookingById(int id)
    {
        return _bookingRepository.GetById(id);
    }

    private int GetFreeUnitNumber(IEnumerable<int> bookedUnits, int countOfUnits)
    {
        if (!bookedUnits.Any())
        {
            return 1;
        }

        int freeUnitNumber = 1;

        for (; freeUnitNumber <= countOfUnits; freeUnitNumber++)
        {
            if (!bookedUnits.Contains(freeUnitNumber))
            {
                return freeUnitNumber;
            }
        }

        return 0;
    }
}