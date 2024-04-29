using MediatR;

namespace CQRS.MediatR.Core.Core.Models
{
    public class GetOrderByIdRequestModel : IRequest<GetOrderByIdResponseModel>
    {
        public Guid OrderId { get; set; }
    }
}
