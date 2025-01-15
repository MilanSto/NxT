using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.Configurations;

internal sealed class ClinicalTrialMetadataConfiguration : IEntityTypeConfiguration<ClinicalTrialMetadata>
{
    public void Configure(EntityTypeBuilder<ClinicalTrialMetadata> builder)
    {
        builder.ToTable("ClinicalTrialMetadata");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(e => e.EndDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.Participants)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion(v => v.ToString(), v => (TrialStatus)Enum.Parse(typeof(TrialStatus), v))
            .HasDefaultValue(TrialStatus.NotStarted);
    }
}