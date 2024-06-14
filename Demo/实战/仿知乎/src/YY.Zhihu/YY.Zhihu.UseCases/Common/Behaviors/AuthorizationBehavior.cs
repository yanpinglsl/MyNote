using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Common.Exceptions;
using YY.Zhihu.UseCases.Common.Interfaces;

namespace YY.Zhihu.UseCases.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>(IUser user) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request
            , RequestHandlerDelegate<TResponse> next
            , CancellationToken cancellationToken)
        {
            var authorizeAttributes = request
                .GetType().GetCustomAttributes<AuthorizeAttribute>();
            if (authorizeAttributes.Any())
            {
                if (user is null || user.Id is null)
                    throw new ForbiddenException();

                // 其它授权逻辑
            }
            return await next();
        }
    }
}
