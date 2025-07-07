namespace AISEA.ApiService.SHARED.DTOs.Responses.Program
{
    public class GetProgramResponse
    {
        public long Id { get; set; }
        public string ProgramCode { get; set; } = null!;
        public string ProgramName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}