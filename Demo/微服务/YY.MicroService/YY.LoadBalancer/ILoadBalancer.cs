namespace YY.LoadBalancer;

public interface ILoadBalancer
{
    string GetNode(List<string> nodes);
}
