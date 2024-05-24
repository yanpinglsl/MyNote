using MediatR;

namespace MediatR_Mediator.API.Events
{
    public class MakeOrderEvent(string uid, string name, int qty) : INotification
    {
        public string Uid { get; set; } = uid;
        public string Name { get; set; } = name;
        public int Qty { get; set; } = qty;
    }
}
