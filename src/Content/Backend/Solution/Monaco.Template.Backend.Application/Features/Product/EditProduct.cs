﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Application.DTOs.Extensions;
using Monaco.Template.Backend.Application.Features.Product.Extensions;
using Monaco.Template.Backend.Application.Infrastructure.Context;
using Monaco.Template.Backend.Application.Services.Contracts;
using Monaco.Template.Backend.Common.Application.Commands;
using Monaco.Template.Backend.Common.Application.Validators.Extensions;
using Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;

namespace Monaco.Template.Backend.Application.Features.Product;

public sealed class EditProduct
{
	public sealed record Command(Guid Id,
								 string Title,
								 string Description,
								 decimal Price,
								 Guid CompanyId,
								 Guid[] Pictures,
								 Guid DefaultPictureId) : CommandBase(Id);

	internal sealed class Validator : AbstractValidator<Command>
	{
		public Validator(AppDbContext dbContext)
		{
			RuleLevelCascadeMode = CascadeMode.Stop;

			this.CheckIfExists<Command, Domain.Model.Product>(dbContext);

			RuleFor(x => x.Title)
				.NotEmpty()
				.MaximumLength(Domain.Model.Product.TitleLength)
				.MustAsync(async (cmd, title, ct) => !await dbContext.ExistsAsync<Domain.Model.Product>(x => x.Id != cmd.Id &&
																											 x.Title == title,
																										ct))
				.WithMessage("Another product with the title {PropertyValue} already exists");

			RuleFor(x => x.Description)
				.NotEmpty()
				.MaximumLength(Domain.Model.Product.DescriptionLength);

			RuleFor(x => x.Price)
				.NotNull()
				.GreaterThanOrEqualTo(0);

			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.MustExistAsync<Command, Domain.Model.Company>(dbContext);

			RuleFor(x => x.Pictures)
				.NotEmpty();

			RuleForEach(cmd => cmd.Pictures)
				.NotEmpty()
				.MustExistAsync<Command, Domain.Model.Image>(dbContext)
				.MustAsync(async (cmd, id, ct) => !await dbContext.ExistsAsync<Domain.Model.Product>(x => x.Id != cmd.Id &&
																										  x.Pictures.Any(p => p.Id == id),
																									 ct))
				.WithMessage("This picture is already in use by another product");

			RuleFor(x => x.DefaultPictureId)
				.NotEmpty()
				.Must((cmd, id) => cmd.Pictures.Contains(id))
				.WithMessage("The default picture must exist in the Pictures array");
		}
	}

	internal sealed class Handler : IRequestHandler<Command, CommandResult>
	{
		private readonly AppDbContext _dbContext;
		private readonly IFileService _fileService;

		public Handler(AppDbContext dbContext, IFileService fileService)
		{
			_dbContext = dbContext;
			_fileService = fileService;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var item = await _dbContext.Set<Domain.Model.Product>()
									   .Include(x => x.Company)
									   .Include(x => x.Pictures)
									   .ThenInclude(x => x.Thumbnail)
									   .SingleAsync(x => x.Id == request.Id, cancellationToken);
			var (company, pictures) = await _dbContext.GetProductData(request.CompanyId, request.Pictures, cancellationToken);

			var (newPics, deletedPics) = request.Map(item, pictures);
			if (company != item.Company)
				company.AddProduct(item);

			_dbContext.Set<Domain.Model.Image>()
					  .RemoveRange(deletedPics);

			await _dbContext.SaveEntitiesAsync(cancellationToken);

			await Task.WhenAll(_fileService.DeleteImagesAsync(deletedPics, cancellationToken),
							   _fileService.MakePermanentImagesAsync(newPics, cancellationToken));

			return CommandResult.Success();
		}
	}
}