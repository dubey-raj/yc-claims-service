using System;
using System.Collections.Generic;

namespace ClaimService.DataStorage.DAO;

public partial class Policy
{
    public long Id { get; set; }

    public string PolicyNumber { get; set; } = null!;

    public long UserId { get; set; }

    public string InsurerName { get; set; } = null!;

    public string? PolicyType { get; set; }

    public decimal? CoverageAmount { get; set; }

    public decimal? DeductibleAmount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
