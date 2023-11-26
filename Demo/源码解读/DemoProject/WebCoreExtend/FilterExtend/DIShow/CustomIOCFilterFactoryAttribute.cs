using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.DIShow
{
    /// <summary>
    /// 基于完成Filter的依赖注入
    /// </summary>
    public class CustomIOCFilterFactoryAttribute : Attribute, IFilterFactory, IFilterMetadata
    {
        private readonly Type _FilterType = null;

        public CustomIOCFilterFactoryAttribute(Type type)
        {
            _FilterType = type;
        }

        //public bool IsReusable => true;//默认是Ture--重用
        public bool IsReusable => false;//改成false

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&");
            Console.WriteLine($"This is {nameof(CustomIOCFilterFactoryAttribute)}.CreateInstance ");

            return (IFilterMetadata)serviceProvider.GetService(_FilterType)!;
        }
    }
}
