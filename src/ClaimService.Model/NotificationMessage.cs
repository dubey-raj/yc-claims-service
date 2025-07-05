namespace ClaimService.Model
{
    public class NotificationEvent<T>
    {
        public string EventType { get; set; }
        public DateTime TimeStamp { get; set; }
        public T Payload { get; set; }
    }

    public class ClaimEvent
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ClaimId { get; set; }
        public string PolicyNumber { get; set; }
        public string IncidentDate { get; set; }
        public string VehicleNumber { get; set; }
        public string SubmissionDate { get; set; }
        public string ReviewDate { get; set; }
        public string ApprovedAmount { get; set; }
        public string ClaimStatus { get; set; }
        public string ReviewRemarks { get; set; }
    }
}
