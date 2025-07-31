using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Approval
{
    public class ApprovalRequest
    {
        public EApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
    }
}
