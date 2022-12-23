using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using VacationRental.Services.Interfaces;
using Xunit;

namespace VacationRental.Api.Tests;

[CollectionDefinition("Integration")]
public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public HttpClient Client { get; }

    public IntegrationFixture()
    {
        _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = new ServiceDescriptor(
                    typeof(IDateProvider),
                    typeof(DateProviderFake),
                    ServiceLifetime.Transient);

                services.Replace(descriptor);
            });
        });

        Client = _factory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}