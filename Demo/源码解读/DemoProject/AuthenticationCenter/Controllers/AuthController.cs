using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AuthenticationCenter.Controllers
{
    /// <summary>
    /// 是为了整合JWT鉴权授权
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        /// <summary>
        /// 就是个简单的webapi---带授权要求，也就是JWT
        /// 1  特性标记
        /// 2  Add+Use
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/Auth/InfoWithAuth")]
        [Authorize]//带个Token就算数---需要灵活授权，集合JWT来拓展下
        public IActionResult InfoWithAuth()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithAuth OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleAdmin")]
        [Authorize(Roles = "Admin")]
        public IActionResult InfoWithRoleAdmin()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleAdmin OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleUser")]
        [Authorize(Roles = "User")]
        public IActionResult InfoWithRoleUser()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleUser OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleAdminOrUser")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult InfoWithRoleAdminOrUser()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleAdminOrUser OK!"
            });
        }

        ////复杂的
        //[HttpGet]
        //[Route("/Auth/InfoWithComplicated")]
        //[Authorize(Policy = "ComplicatedPolicy")]
        //public IActionResult InfoWithComplicated()
        //{
        //    return new JsonResult(new
        //    {
        //        Result = true,
        //        Message = "This is InfoWithComplicated OK!"
        //    });
        //}

        ////动态校验的需求

        //[HttpGet]
        //[Route("/Auth/InfoWithDB")]
        //[Authorize(Policy = "DBPolicy")]
        //public IActionResult InfoWithDB()
        //{
        //    return new JsonResult(new
        //    {
        //        Result = true,
        //        Message = "This is InfoWithDB OK!"
        //    });
        //}
    }
}
