using AutoFixture;
using Monaco.Template.Backend.Common.Tests;
using Monaco.Template.Backend.Domain.Model.Entities;
using File = Monaco.Template.Backend.Domain.Model.Entities.File;

namespace Monaco.Template.Backend.Domain.Tests.Factories.Entities;

public static class DocumentFactory
{
	public static Document Create() =>
		FixtureFactory.Create(f => f.RegisterDocument())
					  .Create<Document>();

	public static IEnumerable<Document> CreateMany() =>
		FixtureFactory.Create(f => f.RegisterDocument())
					  .CreateMany<Document>();
	
	extension(IFixture fixture)
	{
		public IFixture RegisterDocument()
		{
			fixture.Register(() => new Document(fixture.Create<Guid>(),
												fixture.Create<string>(),
												fixture.Create<string>()[..File.ExtensionLength],
												fixture.Create<long>(),
												fixture.Create<string>(),
												false));
			return fixture;
		}
	}
}