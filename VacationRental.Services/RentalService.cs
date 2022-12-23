using VacationRental.Core.Exceptions;
using VacationRental.Data.Entities;
using VacationRental.Data.Interfaces;
using VacationRental.Services.Interfaces;
using static VacationRental.Services.Helpers.BookingDateHelper;

namespace VacationRental.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IDateProvider _dateProvider;

    public RentalService(
        IRentalRepository rentalRepository,
        IBookingRepository bookingRepository,
        IDateProvider dateProvider)
    {
        _rentalRepository = rentalRepository;
        _bookingRepository = bookingRepository;
        _dateProvider = dateProvider;
    }

    public int AddRental(Rental rental)
    {
        return _rentalRepository.Add(rental);
    }

    public Rental GetRentalById(int id)
    {
        return _rentalRepository.GetById(id);
    }

    public void UpdateRental(Rental rental)
    {
        var originalRental = _rentalRepository.GetById(rental.Id);

        if (originalRental == null)
        {
            throw new EntityNotFoundException("Rental not found.");
        }

        var rentalBookings = GetActualRentalBookings(rental);

        if (rental.Units < rentalBookings.Count)
        {
            throw new ApplicationException("Can't update rental because number of active bookings greater then new units number.");
        }

        if (originalRental.PreparationTimeInDays != rental.PreparationTimeInDays)
        {
            foreach (var booking in rentalBookings)
            {
                var updatedBookingEndDate = booking.Start.AddDays(GetRealNightsCount(booking.Nights) + rental.PreparationTimeInDays);

                var overlapedBooking = rentalBookings.Where(b => b.Id != booking.Id && b.Start <= updatedBookingEndDate && b.Unit == booking.Unit);
                if (overlapedBooking.Any())
                {
                    throw new ApplicationException("Can't update rental because new preparation time overlaps with other booking.");
                }
            }

            var bookingsToUpdate = rentalBookings.Select(b =>
            {
                b.PreparationTimeInDays = rental.PreparationTimeInDays;
                return b;
            });

            _bookingRepository.UpdateRange(bookingsToUpdate);
        }

        _rentalRepository.Update(rental);
    }

    private List<Booking> GetActualRentalBookings(Rental rental)
    {
        return _bookingRepository.GetAll().Values.Where(b => b.RentalId == rental.Id && IsBookingActual(b)).ToList();
    }

    private bool IsBookingActual(Booking booking)
    {
        var currentDate = _dateProvider.GetCurrentDate();
        var bookingEndDate = GetBookingEndDate(booking);

        return (currentDate >= booking.Start || currentDate <= booking.Start) && currentDate <= bookingEndDate;
    }
}