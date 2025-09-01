using AISEA.ApiService.SHARED.Const.Enums;
using System.Text.Json.Serialization;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Subject
{
    public class GetSubjectResponse
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<GetSubjectResponse>? Prerequisites { get; set; }
        
        // Approval workflow fields
        public string? CreatedBy { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EApprovalStatus ApprovalStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }

    }
}