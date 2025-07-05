using System;
using System.Collections.Generic;

namespace ClaimService.DataStorage.DAO;

public partial class Claim
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PolicyId { get; set; }

    public DateOnly IncidentDate { get; set; }

    public string IncidentLocation { get; set; } = null!;

    public string? IncidentDescription { get; set; }

    public string ClaimNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? SubmittedAt { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<ClaimAssessment> ClaimAssessments { get; set; } = new List<ClaimAssessment>();

    public virtual ICollection<ClaimDocument> ClaimDocuments { get; set; } = new List<ClaimDocument>();

    public virtual Policy Policy { get; set; } = null!;
}
