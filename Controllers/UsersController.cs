using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private AppDbContext dbContext;
        public UsersController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        
    }
}