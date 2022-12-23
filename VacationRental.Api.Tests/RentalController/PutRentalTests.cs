using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Tests;
using VacationRental.Core.Middleware;
using Xunit;

namespace RentalController;

[Collection("Integration")]
public class PutRentalTests : RentalTestsBase
{
    private readonly HttpClient _client;

    public PutRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task CurrectRequest_ReturnsSuccsessAndUpdatedRental()
    {
        var request = new RentalBindingModel
        {
            Units = 25,
            PreparationTimeInDays = 2
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync(RentalsApiBasePart, request))
        {
            postResponse.IsSuccessStatusCode.Should().BeTrue();
            postResult = await postResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }

        var updateRequest = new RentalBindingUpdateModel
        {
            Id = postResult.Id,
            Units = 5,
            PreparationTimeInDays = 2
        };

        ResourceIdViewModel putResult;
        using var putResponse = await _client.PutAsJsonAsync(RentalsApiBasePart, updateRequest);

        putResponse.IsSuccessStatusCode.Should().BeTrue();
        putResult = await putResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();

        putResult.Should().BeEquivalentTo(postResult);

        var expectedRental = new RentalViewModel
        {
            Id = postResult.Id,
            PreparationTimeInDays = updateRequest.PreparationTimeInDays,
            Units = updateRequest.Units
        };

        using var getResponse = await _client.GetAsync($"{RentalsApiBasePart}/{postResult.Id}");

        getResponse.IsSuccessStatusCode.Should().BeTrue();

        var getResult = await getResponse.Content.ReadFromJsonAsync<RentalViewModel>();

        getResult.Should().BeEquivalentTo(expectedRental);
    }

    [Theory]
    [MemberData(nameof(NegativeRentalTestData))]
    public async Task CorrectRequestWithOwerlapingBooking_ReturnsBadRequestWithMessage(int newUnits, int newPreparationTime, string errorMessage)
    {
        var request = new RentalBindingModel
        {
            Units = 3,
            PreparationTimeInDays = 2
        };

        ResourceIdViewModel postResult;
        using (var postResponse = await _client.PostAsJsonAsync(RentalsApiBasePart, request))
        {
            postResponse.IsSuccessStatusCode.Should().BeTrue();
            postResult = await postResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }

        var postBookingRequest = new BookingBindingModel
        {
            RentalId = postResult.Id,
            Nights = 2,
            Start = new DateTime(2001,01,01)
        };

        await AddBooking(postBookingRequest);

        postBookingRequest.Start = new DateTime(2001, 01, 02);

        await AddBooking(postBookingRequest);

        postBookingRequest.Start = new DateTime(2001, 01, 05);

        await AddBooking(postBookingRequest);

        var updateRequest = new RentalBindingUpdateModel
        {
            Id = postResult.Id,
            Units = newUnits,
            PreparationTimeInDays = newPreparationTime
        };

        using var putResponse = await _client.PutAsJsonAsync(RentalsApiBasePart, updateRequest);

        putResponse.IsSuccessStatusCode.Should().BeFalse();

        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorDetails  = await putResponse.Content.ReadFromJsonAsync<ErrorDetails>();

        errorDetails.Message.Should().Be(errorMessage);

    }

    public static IEnumerable<object[]> NegativeRentalTestData()
    {
        return new List<object[]>
        {
            new object[] {1,2, "Can't update rental because number of active bookings greater then new units number."},
            new object[] {3,5, "Can't update rental because new preparation time overlaps with other booking."}
        };
    }

    private async Task AddBooking(BookingBindingModel booking)
    {
        ResourceIdViewModel postBookingResult;
        using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking))
        {
            postBookingResponse.IsSuccessStatusCode.Should().BeTrue();

            postBookingResult = await postBookingResponse.Content.ReadFromJsonAsync<ResourceIdViewModel>();
        }
    }
}