using AutoFixture;
using Monaco.Template.Backend.Common.Tests;
using Monaco.Template.Backend.Domain.Model.Entities;
using Monaco.Template.Backend.Domain.Model.ValueObjects;
using Moq;

namespace Monaco.Template.Backend.Domain.Tests.Factories.Entities;

public static class AddressFactory
{
	public static Address Create() =>
		FixtureFactory.Create(f => f.RegisterAddress()
									.RegisterCountry())
					  .Create<Address>();

	public static IEnumerable<Address> CreateMany() =>
		FixtureFactory.Create(f => f.RegisterAddress()
									.RegisterCountryMock())
					  .CreateMany<Address>();
}

public static class AddressFactoryExtensions
{
	extension(IFixture fixture)
	{
		public IFixture RegisterAddress()
		{
			fixture.Register(() => new Address(fixture.Create<string?>(),
											   fixture.Create<string?>(),
											   fixture.Create<string?>(),
											   fixture.Create<string?>()?[..Address.PostCodeLength],
											   fixture.Create<Country>()));
			return fixture;
		}

		public IFixture RegisterAddressMock()
		{
			fixture.Register(() =>
							 {
								 var country = fixture.Create<Country>();
								 var mock = new Mock<Address>(fixture.Create<string?>()!,
															  fixture.Create<string?>()!,
															  fixture.Create<string?>()!,
															  fixture.Create<string?>()?[..Address.PostCodeLength]!,
															  country);
								 return mock.Object;
							 });
			return fixture;
		}
	}
}