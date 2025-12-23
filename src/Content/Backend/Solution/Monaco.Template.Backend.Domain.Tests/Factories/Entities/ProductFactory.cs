using AutoFixture;
using Monaco.Template.Backend.Common.Tests;
using Monaco.Template.Backend.Domain.Model.Entities;
using Moq;

namespace Monaco.Template.Backend.Domain.Tests.Factories.Entities;

public static class ProductFactory
{
	public static Product Create() =>
		FixtureFactory.Create(f => f.RegisterImage()
									.RegisterAddress()
									.RegisterCompany()
									.RegisterProduct())
					  .Create<Product>();

	public static IEnumerable<Product> CreateMany() =>
		FixtureFactory.Create(f => f.RegisterImage()
									.RegisterAddress()
									.RegisterCompanyMock()
									.RegisterProductMock())
					  .CreateMany<Product>();
	
	extension(IFixture fixture)
	{
		public IFixture RegisterProduct()
		{
			fixture.Register(() =>
							 {
								 var images = fixture.CreateMany<Image>().ToList();
								 var product = new Product(fixture.Create<string>(),
														   fixture.Create<string>(),
														   fixture.Create<decimal>(),
														   fixture.Create<Company>(),
														   images,
														   images.First());

								 return product;
							 });
			return fixture;
		}

		public IFixture RegisterProductMock()
		{
			fixture.Register(() =>
							 {
								 var images = fixture.CreateMany<Image>().ToList();
								 var company = fixture.Create<Company>();
								 var mock = new Mock<Product>(fixture.Create<string>(),
															  fixture.Create<string>(),
															  fixture.Create<decimal>(),
															  company,
															  images,
															  images.First());
								 mock.SetupGet(x => x.Id)
									 .Returns(Guid.NewGuid());
								 mock.SetupGet(x => x.Company)
									 .Returns(company);
								 mock.SetupGet(x => x.CompanyId)
									 .Returns(company.Id);
								 mock.SetupGet(x => x.Pictures)
									 .Returns([.. images]);
								 mock.SetupGet(x => x.DefaultPicture)
									 .Returns(images.First());

								 return mock.Object;
							 });
			return fixture;
		}
	}
}