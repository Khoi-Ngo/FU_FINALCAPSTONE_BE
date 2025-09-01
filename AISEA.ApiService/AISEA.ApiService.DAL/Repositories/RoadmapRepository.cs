using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories
{
    public class RoadmapRepository : GenericRepository<StudyRoadMap>
    {
        public RoadmapRepository(AiseaContext context) : base(context)
        {
        }
    }
}