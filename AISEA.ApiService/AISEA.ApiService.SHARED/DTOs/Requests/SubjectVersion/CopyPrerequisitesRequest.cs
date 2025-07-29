using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion
{
    public class CopyPrerequisitesRequest
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "FromSubjectVersionId must be a positive number.")]
        public long FromSubjectVersionId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "ToSubjectVersionId must be a positive number.")]
        public long ToSubjectVersionId { get; set; }
    }
}
