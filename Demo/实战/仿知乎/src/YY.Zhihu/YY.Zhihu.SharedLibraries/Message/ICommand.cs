using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.SharedLibraries.Result;

namespace YY.Zhihu.SharedLibraries.Message
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
