using AISEA.ApiService.SHARED.Const.Enums;
using System.Text.Json.Serialization;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Approval
{
    public class ApprovalRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EApprovalStatus ApprovalStatus { get; set; }
        public string? RejectionReason { get; set; }
    }
}
