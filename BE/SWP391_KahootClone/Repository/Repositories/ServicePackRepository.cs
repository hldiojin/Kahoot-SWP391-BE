﻿using Repository.Base;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ServicePackRepository : GenericRepository<ServicePack>
    {
        public ServicePackRepository() { }

        public ServicePackRepository(SWP_KahootContext context) => _context = context;
    }
}
