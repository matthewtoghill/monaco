using AutoFixture;
using Monaco.Template.Backend.Common.Domain.Tests.Factories.Entities;

namespace Monaco.Template.Backend.Common.Domain.Tests.Factories;

public static class FixtureExtensions
{
	extension(IFixture fixture)
	{
		public IFixture RegisterMockFactories() =>
			fixture.RegisterEntityMock()
				   .RegisterEnumerationMock();
	}
}