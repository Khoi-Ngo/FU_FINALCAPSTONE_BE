using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.Extensions
{
    public static class ApprovalStatusExtensions
    {
        public static string GetDisplayName(this EApprovalStatus status)
        {
            return status switch
            {
                EApprovalStatus.PENDING => "Pending",
                EApprovalStatus.APPROVED => "Approved",
                EApprovalStatus.REJECTED => "Rejected",
                _ => "Unknown"
            };
        }

        public static string GetDescription(this EApprovalStatus status)
        {
            return status switch
            {
                EApprovalStatus.PENDING => "Awaiting approval from administrator",
                EApprovalStatus.APPROVED => "Approved by administrator",
                EApprovalStatus.REJECTED => "Rejected by administrator",
                _ => "Status unknown"
            };
        }

        public static bool IsApproved(this EApprovalStatus status)
        {
            return status == EApprovalStatus.APPROVED;
        }

        public static bool IsPending(this EApprovalStatus status)
        {
            return status == EApprovalStatus.PENDING;
        }

        public static bool IsRejected(this EApprovalStatus status)
        {
            return status == EApprovalStatus.REJECTED;
        }
    }
}
