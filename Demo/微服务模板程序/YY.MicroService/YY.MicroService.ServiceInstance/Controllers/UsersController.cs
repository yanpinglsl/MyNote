using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YY.MicroService.Interface;
using YY.MicroService.Model;

namespace YY.MicroService.ServiceInstance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        #region Identity
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _iUserService = null;
        private IConfiguration _iConfiguration;

        public UsersController(ILogger<UsersController> logger
            , IUserService userService
            , IConfiguration configuration)
        {
            _logger = logger;
            this._iUserService = userService;
            this._iConfiguration = configuration;
        }
        #endregion

        [HttpGet]
        [Route("Get")]
        public User Get(int id)
        {
            Console.WriteLine($"This is UsersController {this._iConfiguration["port"]} Invoke Get");
            var host = base.HttpContext!.Request.Host;
            var user = this._iUserService.FindUser(id);
            return new User()
            {
                Id = user.Id,
                Account = user.Account + "MinimalAPI",
                Name = user.Name,
                Role = $"{ this._iConfiguration["ip"] ?? host.Host}{ this._iConfiguration["port"] ?? host.Port!.Value.ToString()}",
                Email = user.Email,
                LoginTime = user.LoginTime,
                Password = user.Password + "K8S"
            };
        }

        [HttpGet]
        [Route("All")]
        public IEnumerable<User> Get()
        {
            Console.WriteLine($"This is UsersController {this._iConfiguration["port"] ?? this._iConfiguration["port"] } Invoke");
            var host = base.HttpContext!.Request.Host;

            return this._iUserService.UserAll().Select(u => new User()
            {
                Id = u.Id,
                Account = u.Account + "MA",
                Name = u.Name,
                Role = $"{ this._iConfiguration["ip"] ?? host.Host}{ this._iConfiguration["port"] ?? host.Port!.Value.ToString()}",
                Email = u.Email,
                LoginTime = u.LoginTime,
                Password = u.Password + "K8S"
            });
        }

    }
}
