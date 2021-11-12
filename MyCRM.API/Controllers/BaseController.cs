using AutoMapper;
using ETLib.Models.QueryResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Services.Services.AccountUserService;
using System.Net;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers
{
    //[Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IMapper Mapper;

        public BaseController(IMapper mapper)
        {
            Mapper = mapper;
            //Init();
        }

        //protected AppUser CurrentUser;

        //protected void Init()
        //{
        //    CurrentUser = _accountUserService.GetCurrentApplicationUserWithData().Result;
        //    if (CurrentUser == null) throw new AuthenticationException();
        //}

#pragma warning disable 1998

        protected async Task<IActionResult> CheckResultAndReturn<T>(ResponseBaseModel<T> response) where T : class
#pragma warning restore 1998
        {
            switch (response.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(response.Model);

                case HttpStatusCode.BadRequest:
                    return BadRequest(response.Message);

                case HttpStatusCode.Created:
                    return StatusCode(201, response.Model);

                case HttpStatusCode.Forbidden:
                    return StatusCode(401);

                case HttpStatusCode.NoContent:
                    return NoContent();

                default:
                    return BadRequest(response.Message);
            }
        }
    }
}