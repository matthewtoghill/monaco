using Monaco.Template.Backend.Application.Features.Company;

namespace Monaco.Template.Backend.Api.DTOs.Extensions;

internal static class CompanyExtensions
{
	extension(CompanyCreateEditDto value)
	{
		public CreateCompany.Command MapCreateCommand() =>
			new(value.Name!,
				value.Email!,
				value.WebSiteUrl!,
				value.Street,
				value.City,
				value.County,
				value.PostCode,
				value.CountryId);

		public EditCompany.Command MapEditCommand(Guid id) =>
			new(id,
				value.Name!,
				value.Email!,
				value.WebSiteUrl!,
				value.Street,
				value.City,
				value.County,
				value.PostCode,
				value.CountryId);
	}
}