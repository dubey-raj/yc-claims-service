using System;
using System.Collections.Generic;

namespace ClaimService.DataStorage.DAO;

public partial class ClaimDocument
{
    public long Id { get; set; }

    public long? ClaimId { get; set; }

    public string? FileUrl { get; set; }

    public string? FileType { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Claim? Claim { get; set; }
}
