using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories
{
    public class StudentProfileRepository : GenericRepository<StudentProfile>
    {
        public StudentProfileRepository(AiseaContext context) : base(context)
        {
        }
    }
}