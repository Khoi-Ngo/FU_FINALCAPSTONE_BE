using System.ComponentModel.DataAnnotations;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment
{
    public class ToggleReactionRequest
    {
        [Required]
        public EReactionType ReactionType { get; set; } // LIKE or UNLIKE
    }
}
