﻿using MediatR;
using Monaco.Template.Backend.Application.DTOs;
using Monaco.Template.Backend.Application.DTOs.Extensions;
using Monaco.Template.Backend.Application.Features.Image.Extensions;
using Monaco.Template.Backend.Application.Infrastructure.Context;
using Monaco.Template.Backend.Common.Application.Queries;

namespace Monaco.Template.Backend.Application.Features.Image;

public sealed class GetImageById
{
	public record Query(Guid Id) : QueryByIdBase<ImageDto>(Id);

	#if filesSupport
	public sealed class Handler : IRequestHandler<Query, ImageDto?>
	{
		private readonly AppDbContext _dbContext;

		public Handler(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<ImageDto?> Handle(Query request, CancellationToken cancellationToken)
		{
			var item = await _dbContext.GetImage(request.Id, cancellationToken);
			return item.Map();
		}
	}
	#endif
}