using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.StudyRoadmap;

public class RoadmapService
{
    private readonly RoadmapRepository _roadmapRepository;

    public RoadmapService(RoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<RoadmapDto> CreateRoadmapAsync(long studentId, string name)
    {
        var entity = await _roadmapRepository.CreateRoadmapAsync(studentId, name);
        return MapToDto(entity);
    }

    public Task<bool> DeleteRoadmapAsync(long roadmapId) =>
        _roadmapRepository.DeleteRoadmapAsync(roadmapId);

    public async Task<RoadmapNodeDto> CreateNodeAsync(long roadmapId, CreateNodeDto dto)
    {
        var entity = new StudyRoadMapNode
        {
            SubjectCode = dto.SubjectCode,
            SemesterNumber = dto.SemesterNumber,
            SubjectName = dto.SubjectName,
            Description = dto.Description,
            IsInternalSubjectData = dto.IsInternalSubjectData,
            StudyRoadMapId = roadmapId
        };

        var created = await _roadmapRepository.CreateNodeAsync(roadmapId, entity);
        return MapToNodeDto(created);
    }

    public Task<bool> DeleteNodeAsync(long nodeId) =>
        _roadmapRepository.DeleteNodeAsync(nodeId);

    public async Task<RoadmapDto?> GetRoadmapAsGraphDtoAsync(long roadmapId)
    {
        var entity = await _roadmapRepository.GetRoadmapWithGraphAsync(roadmapId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<RoadmapLinkDto?> CreateLinkAsync(long fromNodeId, long toNodeId)
    {
        var link = await _roadmapRepository.CreateLinkAsync(fromNodeId, toNodeId);
        if (link == null) return null;

        return new RoadmapLinkDto
        {
            Id = link.Id,
            FromNodeId = link.FromNodeId,
            ToNodeId = link.ToNodeId
        };
    }

    public Task<bool> DeleteLinkAsync(long linkId) =>
        _roadmapRepository.DeleteLinkAsync(linkId);



    // View node
    public async Task<RoadmapNodeDto?> GetNodeAsync(long nodeId)
    {
        var entity = await _roadmapRepository.GetNodeByIdAsync(nodeId);
        if (entity == null) return null;
        return MapToNodeDto(entity); // links will be empty
    }

    // Update node
    public async Task<RoadmapNodeDto?> UpdateNodeAsync(long nodeId, CreateNodeDto dto)
    {
        var node = new StudyRoadMapNode
        {
            Id = nodeId,
            SubjectCode = dto.SubjectCode,
            SemesterNumber = dto.SemesterNumber,
            SubjectName = dto.SubjectName,
            Description = dto.Description,
            IsInternalSubjectData = dto.IsInternalSubjectData
        };

        var updated = await _roadmapRepository.UpdateNodeAsync(node);
        if (updated == null) return null;

        return MapToNodeDto(updated);
    }

    public async Task<bool> ReplaceAllNodesAsync(long roadmapId, List<CreateNodeDto> nodeDtos)
    {
        var nodes = nodeDtos.Select(dto => new StudyRoadMapNode
        {
            SubjectCode = dto.SubjectCode,
            SemesterNumber = dto.SemesterNumber,
            SubjectName = dto.SubjectName,
            Description = dto.Description,
            IsInternalSubjectData = dto.IsInternalSubjectData,
            StudyRoadMapId = roadmapId
        }).ToList();

        return await _roadmapRepository.ReplaceNodesInRoadmapAsync(roadmapId, nodes);
    }



    public async Task<bool> BulkInsertLinksAsync(List<(long FromNodeId, long ToNodeId)> links)
    {
        return await _roadmapRepository.AddLinksToRoadmapAsync(links);
    }


    #region Mapping
    private static RoadmapDto MapToDto(StudyRoadMap entity)
    {
        var links = entity.Nodes
            .SelectMany(n => n.Dependents)
            .Select(d => new RoadmapLinkDto
            {
                Id = d.Id,
                FromNodeId = d.FromNodeId,
                ToNodeId = d.ToNodeId
            }).ToList();

        var nodes = entity.Nodes.Select(n => new RoadmapNodeDto
        {
            Id = n.Id,
            SubjectCode = n.SubjectCode,
            SemesterNumber = n.SemesterNumber,
            SubjectName = n.SubjectName,
            Description = n.Description,
            PrerequisiteIds = n.Prerequisites.Select(p => p.FromNodeId).ToList(),
            DependentIds = n.Dependents.Select(d => d.ToNodeId).ToList(),
            OutgoingLinks = n.Dependents
                .Select(d => new RoadmapLinkDto
                {
                    Id = d.Id,
                    FromNodeId = d.FromNodeId,
                    ToNodeId = d.ToNodeId
                }).ToList()
        }).ToList();

        return new RoadmapDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StudentProfileId = entity.StudentProfileId,
            Nodes = nodes,
            Links = links
        };
    }

    private static RoadmapNodeDto MapToNodeDto(StudyRoadMapNode node)
    {
        return new RoadmapNodeDto
        {
            Id = node.Id,
            SubjectCode = node.SubjectCode,
            SemesterNumber = node.SemesterNumber,
            SubjectName = node.SubjectName,
            Description = node.Description,
            PrerequisiteIds = node.Prerequisites.Select(p => p.FromNodeId).ToList(),
            DependentIds = node.Dependents.Select(d => d.ToNodeId).ToList(),
            OutgoingLinks = node.Dependents
                .Select(d => new RoadmapLinkDto
                {
                    Id = d.Id,
                    FromNodeId = d.FromNodeId,
                    ToNodeId = d.ToNodeId
                }).ToList()
        };
    }
    #endregion
}



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
