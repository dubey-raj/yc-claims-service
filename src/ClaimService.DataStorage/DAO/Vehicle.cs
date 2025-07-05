using System;
using System.Collections.Generic;

namespace ClaimService.DataStorage.DAO;

public partial class Vehicle
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string RegistrationNumber { get; set; } = null!;

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int? Year { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
