using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion
{
    public class AddSubjectVersionPrerequisiteRequest
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "PrerequisiteSubjectVersionId must be a positive number.")]
        public long PrerequisiteSubjectVersionId { get; set; }
    }
}
