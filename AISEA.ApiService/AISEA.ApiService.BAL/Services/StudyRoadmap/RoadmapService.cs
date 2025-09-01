using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.StudyRoadmap
{
    public class RoadmapService
    {
        private readonly RoadmapRepository _roadmapRepository;

        public RoadmapService(RoadmapRepository roadmapRepository)
        {
            _roadmapRepository = roadmapRepository;
        }
    }
}