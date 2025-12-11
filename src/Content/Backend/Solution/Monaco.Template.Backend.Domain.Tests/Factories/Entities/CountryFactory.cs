using AutoFixture;
using Monaco.Template.Backend.Common.Tests;
using Monaco.Template.Backend.Domain.Model.Entities;
using Moq;

namespace Monaco.Template.Backend.Domain.Tests.Factories.Entities;

public static class CountryFactory
{
	public static Country Create() =>
		FixtureFactory.Create(f => f.RegisterCountry())
					  .Create<Country>();

	public static IEnumerable<Country> CreateMany() =>
		FixtureFactory.Create(f => f.RegisterCountryMock())
					  .CreateMany<Country>();
	
	extension(IFixture fixture)
	{
		public IFixture RegisterCountry()
		{
			fixture.Register(() => new Country(fixture.Create<string>()));

			return fixture;
		}

		public IFixture RegisterCountryMock()
		{
			fixture.Register(() =>
							 {
								 var mock = new Mock<Country>(fixture.Create<string>());
								 mock.SetupGet(x => x.Id).Returns(Guid.NewGuid());
								 return mock.Object;
							 });
			return fixture;
		}
	}
}