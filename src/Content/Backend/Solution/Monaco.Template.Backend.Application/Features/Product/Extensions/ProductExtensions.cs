using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Application.Features.Company.DTOs;
using Monaco.Template.Backend.Application.Features.Company.Extensions;
using Monaco.Template.Backend.Application.Features.File.Extensions;
using Monaco.Template.Backend.Application.Features.Product.DTOs;
using Monaco.Template.Backend.Application.Persistence;
using Monaco.Template.Backend.Domain.Model.Entities;
#if massTransitIntegration
using Monaco.Template.Backend.Messages.V1;
#endif
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Application.Features.Product.Extensions;

public static class ProductExtensions
{
	extension(Domain.Model.Entities.Product? value)
	{
		public ProductDto? Map(bool expandCompany = false,
							   bool expandPictures = false,
							   bool expandDefaultPicture = false) =>
			value is null
				? null
				: new(value.Id,
					  value.Title,
					  value.Description,
					  value.Price,
					  value.CompanyId,
					  expandCompany
						  ? value.Company
								 .Map()
						  : null,
					  expandPictures
						  ? value.Pictures
								 .Select(x => x.Map()!)
								 .ToArray()
						  : null,
					  value.DefaultPictureId,
					  expandDefaultPicture
						  ? value.DefaultPicture
								 .Map()
						  : null);
	}
#if (massTransitIntegration)
	
	extension(Domain.Model.Entities.Product item)
	{
		internal ProductCreated MapMessage() =>
			new(item.Id,
				item.Title,
				item.Description,
				item.Price,
				item.CompanyId);
	}
#endif

	extension(AppDbContext dbContext)
	{
		internal async Task<(Domain.Model.Entities.Company company , Image[] pics)> GetProductData(Guid companyId,
																								   Guid[] pictures,
																								   CancellationToken cancellationToken)
		{
			var company = await dbContext.Set<Domain.Model.Entities.Company>()
										 .SingleAsync(x => x.Id == companyId, cancellationToken);
			var pics = await dbContext.Set<Image>()
									  .Include(x => x.Thumbnail)
									  .Where(x => ((IEnumerable<Guid>)pictures).Contains(x.Id))
									  .ToArrayAsync(cancellationToken);
			return (company, pics);
		}
	}

	public static Dictionary<string, Expression<Func<Domain.Model.Entities.Product, object>>> GetMappedFields() =>
		new()
		{
			[nameof(ProductDto.Id)] = x => x.Id,
			[nameof(ProductDto.Title)] = x => x.Title,
			[nameof(ProductDto.Description)] = x => x.Description,
			[nameof(ProductDto.Price)] = x => x.Price,
			[nameof(ProductDto.CompanyId)] = x => x.CompanyId,
			[$"{nameof(ProductDto.Company)}.{nameof(CompanyDto.Name)}"] = x => x.Company.Name,
			[nameof(ProductDto.DefaultPictureId)] = x => x.DefaultPictureId
		};
}