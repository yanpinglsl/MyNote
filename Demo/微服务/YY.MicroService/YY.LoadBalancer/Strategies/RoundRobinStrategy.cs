namespace YY.LoadBalancer.Strategies;

public class RoundRobinStrategy : ILoadBalancingStrategy
{
    private int _index;

    public string Resolve(List<string> nodes)
    {
        if (nodes.Count == 0)
        {
            throw new InvalidOperationException("无可用节点");
        }

        // count=3，index=0
        // _index=1，1 % 3=1
        // _index=2, 2 % 3=2
        // _index=3，3 % 3=0
        // _index=1, 1 % 3=1
        _index = Interlocked.Increment(ref _index) % nodes.Count;
        return nodes[_index];

    }
}
