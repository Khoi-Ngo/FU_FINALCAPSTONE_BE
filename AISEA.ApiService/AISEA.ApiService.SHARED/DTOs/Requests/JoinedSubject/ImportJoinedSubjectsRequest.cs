namespace AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;

public class ImportJoinedSubjectsRequest
{
   public Dictionary<string, HashSet<ImportJoinedSubjects_Data>> UserNameToSubjectsMap { get; set; }
}