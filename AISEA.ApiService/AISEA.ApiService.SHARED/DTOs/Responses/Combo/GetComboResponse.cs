using AISEA.ApiService.SHARED.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Combo
{
    public class GetComboResponse
    {
        public long Id { get; set; }
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public int SubjectCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Approval workflow fields
        public string? CreatedBy { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class GetComboDetailResponse : GetComboResponse
    {
        public List<ComboSubjectResponse> Subjects { get; set; } = new();
    }

    public class ComboSubjectResponse
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
    }
}