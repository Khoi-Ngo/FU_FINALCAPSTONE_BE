namespace AISEA.ApiService.SHARED.DTOs.Roadmap
{

    public class RoadmapDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StudentProfileId { get; set; }
        public List<RoadmapNodeDto> Nodes { get; set; } = new();
        public List<RoadmapLinkDto> Links { get; set; } = new();
    }

    public class RoadmapNodeDto
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; }
        public int? SemesterNumber { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }

        public List<long> PrerequisiteIds { get; set; } = new();
        public List<long> DependentIds { get; set; } = new();

        // Outgoing links from this node (frontend can use directly)
        public List<RoadmapLinkDto> OutgoingLinks { get; set; } = new();
    }

    public class RoadmapLinkDto
    {
        public long Id { get; set; }
        public long FromNodeId { get; set; }
        public long ToNodeId { get; set; }
    }

    public class CreateNodeDto
    {
        public string SubjectCode { get; set; }
        public int? SemesterNumber { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public bool IsInternalSubjectData { get; set; }
    }
}
