using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Enums;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class ClinicalTrialMetadata : Entity
{
    [SetsRequiredMembers]
    public ClinicalTrialMetadata(Guid id, string title, DateTime startDate, DateTime? endDate, int participants, TrialStatus status, int durationInDays)
        : base(id)
    {
        Title = title;
        StartDate = startDate;
        EndDate = endDate;
        Participants = participants;
        Status = status;
        DurationInDays = durationInDays;
    }

    private ClinicalTrialMetadata()
    {
    }

    public string Title { get; private set; }

    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public required int Participants { get; set; }
    public required TrialStatus Status { get; set; }

    public int DurationInDays { get; private set; }
}