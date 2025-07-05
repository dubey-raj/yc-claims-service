namespace ClaimService.Model.APIResponses
{
    public class UserAvailabilityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Region { get; set; } = default!;
        public int CurrentAssignments { get; set; }
    }
}
