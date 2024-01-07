using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace AbpExceptionDemo.Controllers
{
    [Route("api/{controller}")]
    public class Sample06Controller : AbpController
    {
        public ObjectResult Get(string exceptionType)
        {
            switch (exceptionType)
            {
                // ABP初始化 = 500
                case "AbpInitializationException":
                    throw new AbpInitializationException();

                // DTO 模型验证 = 400
                case "AbpValidationException":
                    throw new AbpValidationException();

                // 仓储 get 方法，实体没找到=404
                case "EntityNotFoundException":
                    throw new EntityNotFoundException();

                // 权限 ==  401/403
                case "AbpAuthorizationException":
                    throw new AbpAuthorizationException();

                default:
                    return Ok(new {exceptionType});
            }
        }
    }
}
