using Microsoft.AspNetCore.Routing.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.RouteExtend
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomRouteEndpointExtensions
    {
        public static string Format(this RoutePattern pattern)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"RawText展示文字:{pattern.RawText} <br/>");
            builder.AppendLine($"InboundPrecedence入栈路由优先级:{pattern.InboundPrecedence}<br/>"); //入栈路由（Inbound Routing）：匹配URL
            builder.AppendLine($"OutboundPrecedence出栈路由优先级:{pattern.OutboundPrecedence}<br/>");//出栈路由（Outbound Routing）：生成URL
            var segments = pattern.PathSegments;//分段  用斜线
            builder.AppendLine("Segments分段<br/>");
            foreach (var segment in segments)
            {
                foreach (var part in segment.Parts)
                {
                    builder.AppendLine($"\t{part.Format()} <br/>");
                }
            }
            builder.AppendLine("Defaults 默认值<br/>");
            foreach (var @default in pattern.Defaults)
            {
                builder.AppendLine($"\t{@default.Key} = {@default.Value} <br/>");
            }
            builder.AppendLine("Parameters 参数<br/>");
            foreach (var parameter in pattern.Parameters)
            {
                builder.AppendLine($"\t{parameter.Name}<br/>");
            }

            builder.AppendLine("ParameterPolicies 参数策略<br/>");
            foreach (var policy in pattern.ParameterPolicies)
            {
                builder.AppendLine($"\t{policy.Key} = {string.Join(',', policy.Value.Select(it => it.Content))}");
            }

            return builder.ToString();
        }

        private static string Format(this RoutePatternPart part)
        {
            if (part is RoutePatternLiteralPart literal)
            {
                return $"静态Literal: {literal.Content}<br/>";
            }
            else if (part is RoutePatternSeparatorPart separator)
            {
                return $"分隔符Separator: {separator.Content}<br/>";
            }
            else
            {
                var parameter = (RoutePatternParameterPart)part;
                return $"参数Parameter: Name = {parameter.Name}; Default = {parameter.Default};IsOptional = { parameter.IsOptional};IsCatchAll = { parameter.IsCatchAll};ParameterKind = { parameter.ParameterKind}<br/>";
            }
        }
    }
}
