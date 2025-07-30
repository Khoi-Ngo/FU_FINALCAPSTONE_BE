namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class AddSubjectToCurriculumRequest
    {
        public long SubjectVersionId { get; set; }
        public int SemesterNumber { get; set; }
        public bool IsMandatory { get; set; }
    }
}
