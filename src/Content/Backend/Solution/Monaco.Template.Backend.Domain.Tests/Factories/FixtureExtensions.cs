using AutoFixture;
using Monaco.Template.Backend.Domain.Tests.Factories.Entities;

namespace Monaco.Template.Backend.Domain.Tests.Factories;

public static class FixtureExtensions
{
	extension(IFixture fixture)
	{
		public IFixture RegisterEntityFactories() =>
			fixture.RegisterCompany()
				   .RegisterAddress()
#if (!filesSupport)
				   .RegisterCountry();
#else
				   .RegisterCountry()
				   .RegisterDocument()
				   .RegisterImage()
				   .RegisterProduct();
#endif

		public void RegisterMockFactories() =>
			fixture.RegisterCompanyMock()
				   .RegisterAddressMock()
#if (!filesSupport)
				   .RegisterCountryMock();
#else
				   .RegisterCountryMock()
				   .RegisterDocument()
				   .RegisterImage()
				   .RegisterProductMock();
#endif
	}
}