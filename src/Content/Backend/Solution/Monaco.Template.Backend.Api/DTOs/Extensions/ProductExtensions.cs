using Monaco.Template.Backend.Application.Features.Product;

namespace Monaco.Template.Backend.Api.DTOs.Extensions;

internal static class ProductExtensions
{
	extension(ProductCreateEditDto value)
	{
		public CreateProduct.Command Map() =>
			new(value.Title,
				value.Description,
				value.Price,
				value.CompanyId,
				value.Pictures,
				value.DefaultPictureId);

		public EditProduct.Command Map(Guid id) =>
			new(id,
				value.Title,
				value.Description,
				value.Price,
				value.CompanyId,
				value.Pictures,
				value.DefaultPictureId);
	}
}