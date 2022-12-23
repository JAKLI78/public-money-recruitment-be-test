using FluentAssertions;
using Moq;
using VacationRental.Data.Entities;
using VacationRental.Services;
using VacationRental.Services.Interfaces;
using Xunit;

namespace VavationRental.UnitTests;

public class RentalServiceTests : ServiceTestBase
{
    private readonly Mock<IDateProvider> _dateProviderMock = new Mock<IDateProvider>();

    [Theory]
    [MemberData(nameof(RentalCorrectTestData))]
    public void UpdateRental_CorrectConditions_ThrowNoExceptions(int rentalId, int units, int preparationTimeInDays)
    {
        var allRentals = GetRentalMockData();

        _bookingRepositoryMock.Setup(br => br.GetAll()).Returns(GetBookingMockData());
        _rentalRepositoryMock.Setup(rr => rr.GetAll()).Returns(allRentals);
        _rentalRepositoryMock.Setup(rr => rr.GetById(rentalId)).Returns(allRentals[rentalId]);
        _dateProviderMock.Setup(dp => dp.GetCurrentDate()).Returns(GetDate("12/18/2022"));

        var updatedRental = new Rental { Id = rentalId, PreparationTimeInDays = preparationTimeInDays, Units = units };

        var rentalService = new RentalService(_rentalRepositoryMock.Object, _bookingRepositoryMock.Object, _dateProviderMock.Object);

        var exception = Record.Exception(() => rentalService.UpdateRental(updatedRental));

        exception.Should().BeNull();
    }

    public static IEnumerable<object[]> RentalCorrectTestData()
    {
        return new List<object[]>
        {
            new object[] { 1, 2, 2},
            new object[] { 1, 4, 2 },
            new object[] { 1, 3, 1},
            new object[] { 1, 3, 3},
        };
    }

    private IDictionary<int, Booking> GetBookingMockData()
    {
        return new Dictionary<int, Booking>
            {
                { 1, new Booking() {Id = 1, Nights = 3, RentalId = 1, Unit = 1, Start = GetDate("12/17/2022") } },
                { 2, new Booking() {Id = 2, Nights = 3, RentalId = 2, Unit = 1, Start = GetDate("12/22/2022") } },
                { 3, new Booking() {Id = 3, Nights = 3, RentalId = 2, Unit = 2, Start = GetDate("12/23/2022") } },
                { 4, new Booking() {Id = 4, Nights = 3, RentalId = 1, Unit = 3, Start = GetDate("12/18/2022") } },
            };
    }

    private IDictionary<int, Rental> GetRentalMockData()
    {
        return new Dictionary<int, Rental>
            {
                { 1, new Rental() {Id = 1, PreparationTimeInDays = 2, Units = 3}},
                { 2, new Rental() {Id = 2, PreparationTimeInDays = 3, Units = 4}}
            };
    }
}