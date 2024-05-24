using MediatR;

namespace DotNETCAP.Core.Models
{
    public class GetOrderByIdRequestModel : IRequest<GetOrderByIdResponseModel>
    {
        public Guid OrderId { get; set; }
    }
}
