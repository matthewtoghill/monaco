using Monaco.Template.Backend.Application.Features.File.DTOs;
using Monaco.Template.Backend.Domain.Model.Entities;

namespace Monaco.Template.Backend.Application.Features.File.Extensions;

public static class FileExtensions
{
	extension(Image? value)
	{
		public ImageDto? Map() =>
			value is null
				? null
				: new(value.Id,
					  value.Name,
					  value.Extension,
					  value.ContentType,
					  value.Size,
					  value.UploadedOn,
					  value.IsTemp,
					  value.DateTaken,
					  value.Dimensions.Width,
					  value.Dimensions.Height,
					  value.ThumbnailId,
					  value.ThumbnailId.HasValue ? value.Thumbnail.Map() : null);
	}
}