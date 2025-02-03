using Domain.Enums;
using System;

namespace Presentation.DTOs
{
    public class ClinicalTrialMetadataDto
    {
        public string Title { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Participants { get; set; }
        public TrialStatus Status { get; set; }

        public int DurationInDays { get; set; }
    }
}
