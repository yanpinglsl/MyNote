namespace YY.LoadBalancer.Strategies;

public interface ILoadBalancingStrategy
{
    string Resolve(List<string> nodes);
}
