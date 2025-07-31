using AISEA.ApiService.SHARED.Enums;

namespace AISEA.ApiService.SHARED.Extensions
{
    public static class ApprovalStatusExtensions
    {
        public static string GetDisplayName(this ApprovalStatus status)
        {
            return status switch
            {
                ApprovalStatus.Pending => "Pending",
                ApprovalStatus.Approved => "Approved",
                ApprovalStatus.Rejected => "Rejected",
                _ => "Unknown"
            };
        }

        public static string GetDescription(this ApprovalStatus status)
        {
            return status switch
            {
                ApprovalStatus.Pending => "Awaiting approval from administrator",
                ApprovalStatus.Approved => "Approved by administrator",
                ApprovalStatus.Rejected => "Rejected by administrator",
                _ => "Status unknown"
            };
        }

        public static bool IsApproved(this ApprovalStatus status)
        {
            return status == ApprovalStatus.Approved;
        }

        public static bool IsPending(this ApprovalStatus status)
        {
            return status == ApprovalStatus.Pending;
        }

        public static bool IsRejected(this ApprovalStatus status)
        {
            return status == ApprovalStatus.Rejected;
        }
    }
}
