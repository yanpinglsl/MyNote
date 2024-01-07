using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AbpExceptionDemo.Localization.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace AbpExceptionDemo.Controllers
{
    [Route("api/Sample03")]
    public class Sample03Controller : AbpController
    {
        private readonly IStringLocalizer<ExceptionResource> _stringLocalizer;

        public Sample03Controller(IStringLocalizer<ExceptionResource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public ObjectResult Get()
        {
            throw new UserFriendlyException(_stringLocalizer["ThisIsAUserFriendlyException"],"Sample:03");
        }
    }
}
