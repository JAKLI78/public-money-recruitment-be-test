using FluentAssertions;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Tests;
using VacationRental.Core.Middleware;
using Xunit;

namespace BookingController;

[Collection("Integration")]
public class PostBookingTests
{
    private readonly HttpClient _client;

    public PostBookingTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task AddBooking_CorrectRequest_ReturnsNewId()
    {
        var bookingNights = 3;
        var bookingStartDate = new DateTime(2001, 01, 01);
        var rentalPreparationTimeInDays = 2;

        var postRentalRequest = new RentalBindingModel
        {
            PreparationTimeInDays = rentalPreparationTimeInDays,
            Units = 4
        };

        ResourceIdViewModel postRentalResult;
        using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
        {
            postRentalResponse.IsSuccessStatusCode.Should().BeTrue();

            postRentalResult = await postRentalResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }

        var postBookingRequest = new BookingBindingModel
        {
            RentalId = postRentalResult.Id,
            Nights = bookingNights,
            Start = bookingStartDate
        };

        ResourceIdViewModel postBookingResult;
        using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
        {
            postBookingResponse.IsSuccessStatusCode.Should().BeTrue();

            postBookingResult = await postBookingResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }

        var expectedBooking = new BookingViewModel
        {
            Id = postBookingResult.Id,
            Nights = bookingNights,
            Start = bookingStartDate,
            RentalId = postBookingResult.Id,
        };

        using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/{postBookingResult.Id}"))
        {
            getBookingResponse.IsSuccessStatusCode.Should().BeTrue();

            var getBookingResult = await getBookingResponse.Content.ReadFromJsonAsync<BookingViewModel>();

            getBookingResult.Should().BeEquivalentTo(expectedBooking);
        }
    }

    [Fact]
    public async Task CorrectRequestWithOverlapedDate_ReturnsBadRequestWithMessage()
    {
        var bookingNights = 3;
        var bookingStartDate = new DateTime(2002, 01, 01);

        var postRentalRequest = new RentalBindingModel
        {
            Units = 1
        };

        ResourceIdViewModel postRentalResult;
        using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
        {
            postRentalResponse.IsSuccessStatusCode.Should().BeTrue();

            postRentalResult = await postRentalResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }

        var postFirstBookingRequest = new BookingBindingModel
        {
            RentalId = postRentalResult.Id,
            Nights = bookingNights,
            Start = bookingStartDate
        };

        using (var postFirstBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postFirstBookingRequest))
        {
            postFirstBookingResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        var postSecondBookingRequest = new BookingBindingModel
        {
            RentalId = postRentalResult.Id,
            Nights = 1,
            Start = bookingStartDate.AddDays(1)
        };

        using var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postSecondBookingRequest);

        postBooking2Response.IsSuccessStatusCode.Should().BeFalse();
        postBooking2Response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var errorDetails = await postBooking2Response.Content.ReadFromJsonAsync<ErrorDetails>();

        errorDetails.Message.Should().Be("Not available");
    }
}