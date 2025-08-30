using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment
{
    public class CreateSubjectCommentRequest
    {
        [Required(ErrorMessage = "Subject ID is required")]
        public long SubjectId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters")]
        public string Content { get; set; } = null!;


    }
}
