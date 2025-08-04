using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories;

public class JoinedCourseRepository : GenericRepository<JoinedCourse>
{
    public JoinedCourseRepository(AiseaContext context) : base(context)
    {
    }
}