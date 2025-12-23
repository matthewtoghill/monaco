using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monaco.Template.Backend.Application.Persistence.EntityConfigurations.Seeds;
using Monaco.Template.Backend.Common.Infrastructure.EntityConfigurations.Extensions;
using Monaco.Template.Backend.Domain.Model.Entities;

namespace Monaco.Template.Backend.Application.Persistence.EntityConfigurations;

internal sealed class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		builder.ConfigureIdWithValueGeneratedNever();

		builder.Property(x => x.Name)
			   .IsRequired()
			   .HasMaxLength(Country.NameLength);

		builder.HasData(CountrySeed.GetCountries());
	}
}