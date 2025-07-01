namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class AddSubjectToCurriculumRequest
    {
        public long SubjectId { get; set; }
        public int SemesterNumber { get; set; }
        public bool IsMandatory { get; set; }
    }
}