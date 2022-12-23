using Moq;
using System.Globalization;
using VacationRental.Data.Interfaces;

namespace VavationRental.UnitTests;

public abstract class ServiceTestBase
{
    private const string DateFormat = "MM/dd/yyyy";
    protected readonly Mock<IRentalRepository> _rentalRepositoryMock;
    protected readonly Mock<IBookingRepository> _bookingRepositoryMock;

    public ServiceTestBase()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
    }

    protected static DateTime GetDate(string date)
    {
        return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture).Date;
    }
}