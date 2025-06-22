using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.BgService.Worker.Abstract;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Persistence;

namespace AISEA.BgService.Worker.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AiseaContext context) : base(context)
        {
        }

    }
}