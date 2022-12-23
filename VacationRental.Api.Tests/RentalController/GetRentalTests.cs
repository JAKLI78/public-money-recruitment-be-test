using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VacationRental.Api.Tests;
using VacationRental.Core.Middleware;
using Xunit;

namespace RentalController;

[Collection("Integration")]
public class GetRentalTests : RentalTestsBase
{
    private readonly HttpClient _client;

    public GetRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task CorrectRequestWithNotExistingRentalId_ReturnsNotFoundWithMessage()
    {
        using var getResponse = await _client.GetAsync($"{RentalsApiBasePart}/99");

        getResponse.IsSuccessStatusCode.Should().BeFalse();

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorDetails = await getResponse.Content.ReadFromJsonAsync<ErrorDetails>();

        errorDetails.Message.Should().Be("Rental not found");
    }
}