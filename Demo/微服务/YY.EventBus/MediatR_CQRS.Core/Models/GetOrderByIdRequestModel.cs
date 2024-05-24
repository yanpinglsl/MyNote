using MediatR;

namespace MediatR_CQRS.Core.Models
{
    public class GetOrderByIdRequestModel : IRequest<GetOrderByIdResponseModel>
    {
        public Guid OrderId { get; set; }
    }
}
