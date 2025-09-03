using System.Text.Json;
using System.Text.Json.Serialization;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Responses.MarkReport;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using AISEA.ApiService.SHARED.DTOs.Roadmap;
using AISEA.ApiService.SHARED.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AISEA.ApiService.BAL.Services.StudyRoadmap;

public class RoadmapService
{
    private readonly RoadmapRepository _roadmapRepository;
    private readonly RoadmapNodeRepository _roadmapNodeRepository;

    private readonly IChatOpenAIService _chatOpenAIService;
    private readonly IJWTService _jWTService;
    private readonly SubjectRepository _subjectRepository;
    private readonly IRedisRepository _redisRepository;

    public RoadmapService(RoadmapRepository roadmapRepository, RoadmapNodeRepository roadmapNodeRepository, IChatOpenAIService chatOpenAIService, IJWTService jWTService, SubjectRepository subjectRepository, IRedisRepository redisRepository)
    {
        _roadmapRepository = roadmapRepository;
        _roadmapNodeRepository = roadmapNodeRepository;
        _chatOpenAIService = chatOpenAIService;
        _jWTService = jWTService;
        _subjectRepository = subjectRepository;
        _redisRepository = redisRepository;
    }






    #region AI FEATURE
    public async Task<List<CreateNodeDto>> GenNodeAsync(string accessToken, string studentMessage)
    {

        var studentUserId = _jWTService.GetUserIdFromToken(accessToken);
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        var studentPersonalSubjectsInCurriculum = await GetPersonalCurSubjects(studentProfileId);
        var studentDataJSON = await GetStudentDataJSON(studentUserId, jsonOptions);
        var FPTUniversityAcademicResourceDataJSON = await GetFLMJSON(jsonOptions);
        var studentCurrentTranscriptJSON = await GetTranscriptJSON(studentProfileId, jsonOptions);
        var studentPersonalSubjectsInCombo = await GetPersonalComboSubjects(studentProfileId);

        if (studentPersonalSubjectsInCombo.IsNullOrEmpty() && !studentPersonalSubjectsInCurriculum.IsNullOrEmpty())
        {
            //case student  have no combo yet
            //call Open AI to choose the appropriate combo for student
            var promptToGetSuggestedCombo = CallAIConst.TemplatePromptToGetSuggestedComboForStudent
            .Replace("{studentCurrentTranscriptJSON}", studentCurrentTranscriptJSON)
            .Replace("{studentMessage}", studentMessage)
            .Replace("{studentDataJSON}", studentDataJSON)
            .Replace("{FPTUniversityAcademicResourceDataJSON}", FPTUniversityAcademicResourceDataJSON)
            ;



            try
            {


                var comboOfStudent = await _chatOpenAIService.GetSuggestedComboForStudent(promptToGetSuggestedCombo);
                studentPersonalSubjectsInCombo = await _subjectRepository.GetAllViaComboNameAsync(comboOfStudent);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }



        #region Init node list and add internal
        var nodes = new List<CreateNodeDto>();
        var usedSubjectCodes = new HashSet<string>();

        try
        {
            // Add curriculum subjects
            foreach (var subject in studentPersonalSubjectsInCurriculum)
            {

                if (!usedSubjectCodes.Contains(subject.SubjectCode))
                {
                    nodes.Add(new CreateNodeDto
                    {
                        SubjectCode = subject.SubjectCode,
                        SemesterNumber = subject.SemesterNumber,
                        SubjectName = subject.SubjectName,
                        Description = subject.Description,
                        IsInternalSubjectData = true
                    });
                    usedSubjectCodes.Add(subject.SubjectCode);
                }
            }

            // Add combo subjects
            foreach (var comboSubject in studentPersonalSubjectsInCombo)
            {

                if (!usedSubjectCodes.Contains(comboSubject.SubjectCode))
                {
                    nodes.Add(new CreateNodeDto
                    {
                        SubjectCode = comboSubject.SubjectCode,
                        SemesterNumber = comboSubject.SemesterNumber,
                        SubjectName = comboSubject.SubjectName,
                        Description = comboSubject.Description,
                        IsInternalSubjectData = true
                    });
                    usedSubjectCodes.Add(comboSubject.SubjectCode);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        #endregion

        //get the JSON data of current nodes
        var currentNodesJSON = JsonSerializer.Serialize(nodes, jsonOptions);


        var promptToGetExternalSubjectNodes = CallAIConst.TemplateForGenExternaleSubjectNodesForStudent
            .Replace("{studentCurrentTranscriptJSON}", studentCurrentTranscriptJSON)
            .Replace("{studentMessage}", studentMessage)
            .Replace("{studentDataJSON}", studentDataJSON)
            .Replace("{FPTUniversityAcademicResourceDataJSON}", FPTUniversityAcademicResourceDataJSON)
            .Replace("{currentNodesJSON}", currentNodesJSON)
            ;



        try
        {
            var externalSubjectNodes = await _chatOpenAIService.GenExternalSubjectNodesInStudyRoadmap(promptToGetExternalSubjectNodes);

            nodes.AddRange(externalSubjectNodes);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return nodes;

    }

    public async Task<List<RoadmapLinkDto>> GenLinkAsync(RoadmapDto currentRoadmap)
    {

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        var currentRoadmapJSON = JsonSerializer.Serialize(currentRoadmap, jsonOptions);

        var promptToGetSuggestedLinkForCurrentRoadmap = CallAIConst.TemplateToLinkAllNodesPrompt
         .Replace("{currentRoadmapJSON}", currentRoadmapJSON)
         ;

        return await _chatOpenAIService.GetTheCompleteLinkedNodes(promptToGetSuggestedLinkForCurrentRoadmap);
    }



    #endregion


    #region AI FEATURE CACHING HANDLING

    private async Task<string> GetTranscriptJSON(long studentProfileId, JsonSerializerOptions jsonOptions)
    {
        try
        {
            var cacheKey = $"{CacheKeyForAIFeature.PrefixToGetStudentTranscriptByStudentProfileID}{studentProfileId}";
            var res = await _redisRepository.GetValueAsync<List<TranscriptItemResponse>>(cacheKey);
            return JsonSerializer.Serialize(res, jsonOptions);
        }
        catch (Exception e)
        {
            return "{}";
        }
    }

    private async Task<string> GetStudentDataJSON(long studentUserId, JsonSerializerOptions jsonOptions)
    {
        try
        {
            var cacheKey = $"{CacheKeyForAIFeature.PrefixToGetStudentDataByUserID}{studentUserId}";
            var res = await _redisRepository.GetValueAsync<DAL.Entities.User>(cacheKey);
            return JsonSerializer.Serialize(res, jsonOptions);
        }
        catch (Exception e)
        {
            return "{}";
        }
    }

    private async Task<string> GetFLMJSON(JsonSerializerOptions jsonOptions)
    {

        try
        {
            var cacheKey = CacheKeyForAIFeature.PrefixToGetAllDataOfFLMCurComSub;
            var res = await _redisRepository.GetValueAsync<object>(cacheKey);
            return JsonSerializer.Serialize(res, jsonOptions);
        }
        catch (Exception e)
        {
            return "{}";
        }
    }




    private async Task<List<SimpleSubjectResponse>> GetPersonalComboSubjects(long studentProfileId)
    {
        try
        {
            var cacheKey = $"{CacheKeyForAIFeature.PrefixToGetPersonalComboByStudentProfileID}{studentProfileId}";
            return await _redisRepository.GetValueAsync<List<SimpleSubjectResponse>>(cacheKey);
        }
        catch (Exception e)
        {
            return new List<SimpleSubjectResponse>();
        }
    }


    private async Task<List<SimpleSubjectResponse>> GetPersonalCurSubjects(long studentProfileId)
    {

        try
        {
            var cacheKey = $"{CacheKeyForAIFeature.PrefixToGetPersonalCurByStudentProfileID}{studentProfileId}";
            return await _redisRepository.GetValueAsync<List<SimpleSubjectResponse>>(cacheKey);
        }
        catch (Exception e)
        {
            return new List<SimpleSubjectResponse>();
        }
    }

    #endregion

    public async Task<long> GetRoadmapIdAsync(long studentProfileId)
    => await _roadmapRepository.GetIDByStudentProfileIDAsync(studentProfileId);


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
    public async Task UpdateNodeAsync(long nodeId, CreateNodeDto dto)
    {
        var node = await _roadmapNodeRepository.GetByIdAsync(nodeId);
        node.SubjectCode = dto.SubjectCode;
        node.SubjectName = dto.SubjectName;
        node.SemesterNumber = dto.SemesterNumber;
        node.Description = dto.Description;
        node.IsInternalSubjectData = dto.IsInternalSubjectData;


        await _roadmapNodeRepository.UpdateAsync(node);

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

    public async Task<bool> BulkInsertLinksAsync(List<RoadmapLinkDto> links)
    {
        var linkTuples = links.Select(l => (FromNodeId: l.FromNodeId, ToNodeId: l.ToNodeId)).ToList();

        return await _roadmapRepository.AddLinksToRoadmapAsync(linkTuples);
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
            IsInternalSubjectData = node.IsInternalSubjectData,
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


