namespace YY.LoadBalancer.AspNetCore;

public interface ILoadBalancer<T> : ILoadBalancer where T : class;
