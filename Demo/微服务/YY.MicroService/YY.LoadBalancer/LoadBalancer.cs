using YY.LoadBalancer.Strategies;

namespace YY.LoadBalancer;

public class LoadBalancer(LoadBalancingStrategy strategy) : ILoadBalancer
{
    private readonly ILoadBalancingStrategy _strategy = strategy switch
    {
        LoadBalancingStrategy.Random => new RandomStrategy(),
        LoadBalancingStrategy.RoundRobin => new RoundRobinStrategy(),
        _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
    };
    public string GetNode(List<string> nodes)
    {
        return _strategy.Resolve(nodes);
    }
}
