using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyCRM.API.Controllers
{
    [Route("api/background")]
    [ApiController]
    public class BackgroundWorkController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("warm")]
        public async Task<IActionResult> Timed()
        {
            return Ok();
        }
    }
}