using AwesomeAssertions;
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Application.Features.Country.DTOs;
using Monaco.Template.Backend.Domain.Model.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Monaco.Template.Backend.IntegrationTests.Tests;

[ExcludeFromCodeCoverage]
[Collection("IntegrationTests")]
[Trait("Integration Tests", "Countries")]
public class CountriesTests : IntegrationTest
{
	public CountriesTests(AppFixture fixture) : base(fixture)
	{ }

#if (apiService && auth)
	protected override bool RequiresAuthentication => true;
#endif

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
#if (auth)

		await SetupAccessToken([]);
#endif
	}

	[Fact(DisplayName = "Get Countries succeeds")]
	public async Task GetCountriesSucceeds()
	{
		using var client = GetClient(Fixture.WebAppFactory);
		var response = await client.Request(ApiRoutes.Countries.Query())
								   .GetAsync();

		response.StatusCode
				.Should()
				.Be((int)HttpStatusCode.OK);

		var result = await response.GetJsonAsync<CountryDto[]>();
		var countriesCount = await Fixture.GetDbContext(Fixture.WebAppFactory.Services)
										  .Set<Country>()
										  .CountAsync();

		result.Should()
			  .NotBeNull();
		result.Should()
			  .HaveCount(countriesCount);
	}

	[Fact(DisplayName = "Get Country succeeds")]
	public async Task GetCountrySucceeds()
	{
		var countryId = Guid.Parse("534A826B-70EF-2128-1A4C-52E23B7D5447");

		using var client = GetClient(Fixture.WebAppFactory);
		var response = await client.Request(ApiRoutes.Countries.Get(countryId))
								   .GetAsync();

		response.StatusCode
				.Should()
				.Be((int)HttpStatusCode.OK);

		var result = await response.GetJsonAsync<CountryDto>();
		var country = await Fixture.GetDbContext(Fixture.WebAppFactory.Services)
								   .Set<Country>()
								   .SingleAsync(c => c.Id == countryId);

		result.Should()
			  .NotBeNull();
		result.Name
			  .Should()
			  .Be(country.Name);
	}
}