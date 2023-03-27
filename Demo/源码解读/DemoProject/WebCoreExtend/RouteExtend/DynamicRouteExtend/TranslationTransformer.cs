using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace WebCoreExtend.RouteExtend.DynamicRouteExtend
{
    public class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly CustomTranslationSource _CustomTranslationSource;
        public TranslationTransformer(CustomTranslationSource translationSource)
        {
            this._CustomTranslationSource = translationSource;
        }
        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext
       , RouteValueDictionary values)
        {
            if (!values.ContainsKey("language")
                || !values.ContainsKey("controller")
                || !values.ContainsKey("action"))
            {
                return values;
            }

            var language = values["language"]?.ToString();

            var controller = await this._CustomTranslationSource.Mapping(language,
                values["controller"]?.ToString());
            if (controller == null) return values;
            values["controller"] = controller;


            var action = await this._CustomTranslationSource.Mapping(language,
                values["action"]?.ToString());
            if (action == null) return values;
            values["action"] = action;

            return values;
        }
    }
}
