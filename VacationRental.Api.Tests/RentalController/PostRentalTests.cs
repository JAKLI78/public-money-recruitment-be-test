using FluentAssertions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using VacationRental.Api.Tests;
using Xunit;

namespace RentalController;

[Collection("Integration")]
public class PostRentalTests : RentalTestsBase
{
    private readonly HttpClient _client;

    public PostRentalTests(IntegrationFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task CorrectRequest_CreatesRental()
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

        var expectedRental = new RentalViewModel
        {
            Id = postResult.Id,
            PreparationTimeInDays = request.PreparationTimeInDays,
            Units = request.Units
        };

        using var getResponse = await _client.GetAsync($"{RentalsApiBasePart}/{postResult.Id}");

        getResponse.IsSuccessStatusCode.Should().BeTrue();

        var getResult = await getResponse.Content.ReadFromJsonAsync<RentalViewModel>();

        getResult.Should().BeEquivalentTo(expectedRental);
    }
}