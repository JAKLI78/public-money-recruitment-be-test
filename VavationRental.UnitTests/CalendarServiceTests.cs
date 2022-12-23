using FluentAssertions;
using VacationRental.Core.Exceptions;
using VacationRental.Data.Entities;
using VacationRental.Services;
using VacationRental.Services.Models;
using Xunit;

namespace VavationRental.UnitTests;

public class CalendarServiceTests : ServiceTestBase
{
    private static string StartDateString = "12/20/2022";

    [Theory]
    [MemberData(nameof(GetCalendarTestData))]
    public void GetCalendarForRental_CorrectConditions_Returns_CorrectCalendar(int rentalId, int countOfNights, CalendarModel expected)
    {
        var allRentals = GetRentalMockData();

        var startDate = GetDate(StartDateString);

        _bookingRepositoryMock.Setup(br => br.GetAll()).Returns(GetBookingMockData());
        _rentalRepositoryMock.Setup(rr => rr.GetAll()).Returns(allRentals);
        _rentalRepositoryMock.Setup(rr => rr.GetById(rentalId)).Returns(allRentals[rentalId]);

        var calendarService = new CalendarService(_rentalRepositoryMock.Object, _bookingRepositoryMock.Object);

        var result = calendarService.GetCalendarForRental(rentalId, startDate, countOfNights);

        result.RentalId.Should().Be(rentalId);
        result.Dates.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetCalendarForRental_NotExistingRental_ThrowsEntityNotFoundException()
    {
        var notExistedRentalId = 3;

        var startDate = GetDate(StartDateString);

        _rentalRepositoryMock.Setup(rr => rr.GetById(notExistedRentalId)).Returns((Rental)null);

        var calendarService = new CalendarService(_rentalRepositoryMock.Object, _bookingRepositoryMock.Object);

        var exception = Record.Exception(() => calendarService.GetCalendarForRental(notExistedRentalId, startDate, 1));

        exception.Should().BeOfType<EntityNotFoundException>();

        var notFoundExeption = exception as EntityNotFoundException;
        notFoundExeption.Should().NotBeNull();

        notFoundExeption.Message.Should().Be("Rental with such id not found.");
    }

    private IDictionary<int, Booking> GetBookingMockData()
    {
        return new Dictionary<int, Booking>
        {
            { 1, new Booking() {Id = 1, Nights = 3, RentalId = 1, Unit = 1, Start = GetDate("12/17/2022") } },
            { 2, new Booking() {Id = 2, Nights = 3, RentalId = 2, Unit = 1, Start = GetDate("12/22/2022") } },
            { 3, new Booking() {Id = 3, Nights = 3, RentalId = 2, Unit = 2, Start = GetDate("12/23/2022") } },
            { 4, new Booking() {Id = 4, Nights = 3, RentalId = 1, Unit = 2, Start = GetDate("12/18/2022") } },
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

    public static IEnumerable<object[]> GetCalendarTestData()
    {
        var startDate = GetDate(StartDateString);

        return new List<object[]>
        {
            new object[] {
                1,
                5,
                new CalendarModel
                {
                    RentalId = 1,
                    Dates = new List<CalendarDateModel>
                    {
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>
                            {
                                new CalendarBookingModel { Id = 4, Unit = 2 }
                            },
                            Date = startDate,
                            PreparationTimes = new List<CalendarPreparationModel>
                            {
                                new CalendarPreparationModel { Unit = 1 }
                            },
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel> { },
                            Date = startDate.AddDays(1),
                            PreparationTimes = new List<CalendarPreparationModel>
                            {
                                new CalendarPreparationModel { Unit = 1 },
                                new CalendarPreparationModel { Unit = 2 }
                            },
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel> { },
                            Date = startDate.AddDays(2),
                            PreparationTimes = new List<CalendarPreparationModel>
                            {
                                new CalendarPreparationModel { Unit = 2 }
                            },
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel> { },
                            Date = startDate.AddDays(3),
                            PreparationTimes = new List<CalendarPreparationModel> { },
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel> { },
                            Date = startDate.AddDays(4),
                            PreparationTimes = new List<CalendarPreparationModel> { },
                        },
                    }
                }
            },
            new object[]
            {
                2,
                7,
                new CalendarModel
                {
                    RentalId = 2,
                    Dates = new List<CalendarDateModel>
                    {
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>(),
                            Date = startDate,
                            PreparationTimes = new List<CalendarPreparationModel>(),
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>(),
                            Date = startDate.AddDays(1),
                            PreparationTimes = new List<CalendarPreparationModel>(),
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>
                            {
                                new CalendarBookingModel { Id = 2, Unit = 1 }
                            },
                            Date = startDate.AddDays(2),
                            PreparationTimes = new List<CalendarPreparationModel>(),
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>
                            {
                                new CalendarBookingModel { Id = 2, Unit = 1 },
                                new CalendarBookingModel { Id = 3, Unit = 2 }
                            },
                            Date = startDate.AddDays(3),
                            PreparationTimes = new List<CalendarPreparationModel>(),
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>
                            {
                                new CalendarBookingModel { Id = 2, Unit = 1 },
                                new CalendarBookingModel { Id = 3, Unit = 2 }
                            },
                            Date = startDate.AddDays(4),
                            PreparationTimes = new List<CalendarPreparationModel>(),
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>
                            {
                                new CalendarBookingModel { Id = 3, Unit = 2 }
                            },
                            Date = startDate.AddDays(5),
                            PreparationTimes = new List<CalendarPreparationModel>
                            {
                                new CalendarPreparationModel{ Unit = 1 }
                            },
                        },
                        new CalendarDateModel()
                        {
                            Bookings = new List<CalendarBookingModel>(),
                            Date = startDate.AddDays(6),
                            PreparationTimes = new List<CalendarPreparationModel>
                            {
                                new CalendarPreparationModel{ Unit = 1 },
                                new CalendarPreparationModel{ Unit = 2 }
                            },
                        },
                    }
                }
            }
        };
    }
}