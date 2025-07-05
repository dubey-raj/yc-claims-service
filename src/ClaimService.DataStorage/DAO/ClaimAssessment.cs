using System;
using System.Collections.Generic;

namespace ClaimService.DataStorage.DAO;

public partial class ClaimAssessment
{
    public long Id { get; set; }

    public long ClaimId { get; set; }

    public long InspectorId { get; set; }
    public long ManagerId { get; set; }

    public DateOnly? InspectionDate { get; set; }

    public string? DamageAssessed { get; set; }

    public string? ReviewNote { get; set; }

    public decimal? EstimatedAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Claim Claim { get; set; } = null!;
}
