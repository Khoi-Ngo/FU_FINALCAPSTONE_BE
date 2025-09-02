using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories;

public class RoadmapNodeRepository : GenericRepository<StudyRoadMapNode>
{
    public RoadmapNodeRepository(AiseaContext context) : base(context)
    {
    }
    
}