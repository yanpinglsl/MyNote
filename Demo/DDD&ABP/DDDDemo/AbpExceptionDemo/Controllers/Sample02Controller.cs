using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Localization;
using Volo.Abp.Validation;

namespace AbpExceptionDemo.Controllers
{
    [Route("api/Sample02")]
    public class Sample02Controller : AbpController
    {
        public ObjectResult Get()
        {
            throw new UserFriendlyException("这是一个用户友好异常","Sample:02");
        }
    }
}
