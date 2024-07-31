# 微服务组件之Polly

## 服务雪崩

### 定义

​	在微服务架构中，当中间某一层的服务故障，则会导致上层服务的请求堆积，系统硬盘/CPU资源耗尽，从而会导致上游一连串的服务故障，而导致服务雪崩。即一个服务失败，导致整条链路的服务都失败。

### 产生原因

- 依赖关系复杂性： 在微服务架构中，各个服务之间存在复杂的依赖关系。如果一个服务出现故障，它可能会导致依赖于它的其他服务也无法正常工作。
- 大规模部署： 大规模部署意味着有大量的服务实例在运行，当其中一部分实例出现问题时，整个系统可能受到影响。
- 同时故障： 有时，多个服务可能因相同的原因（如硬件故障、网络问题或配置错误）而同时故障，导致雪崩效应。
- 超时和重试： 如果某个服务在请求时长时间未响应，其他服务可能会发起重试请求，导致更多的负载，最终导致系统崩溃。
- 资源耗尽： 当某个服务的资源（如数据库连接、线程池）被过度消耗时，它可能会无法响应请求，从而引发雪崩。

### 解决方案

雪崩解决常见解决方案有以下几种:

- 超时处理：对于每个微服务的请求，应该设置合理的超时时间。超时时间应该充分考虑服务的响应时间和业务需求，以避免等待时间过长导致的问题
- 舱壁模式（Bulkhead Pattern for Avalanche）:系统遇到雪崩风险时，通过隔离不同服务或组件，以防止一个故障或高负载情况影响整个系统的稳定性。是一种应对潜在雪崩的设计模式
- 限流（Rate Limiting）: 限流可以控制对服务的请求速率，确保不会超出服务的处理能力。这可以防止流量过多而导致系统崩溃
- 熔断器模式（Circuit Breaker Pattern）：熔断器模式是一种容错模式，用于避免雪崩效应。熔断器会监控服务的健康状态，当服务连续出现故障或响应时间超过阈值时，熔断器会打开，阻止进一步的请求流量流向该服务，从而保护系统的稳定性
- 降级策略（Fallback）： 降级是一种处理服务不可用或性能下降的策略，它允许系统在出现问题时提供有限但稳定的功能，而不是完全失败。当服务出现问题时，降级策略可以返回默认值、缓存数据、执行备用操作或者提供一个基本的响应，以确保用户仍然能够访问系统的一部分功能

#### 超时处理

针对服务调用增加超时机制(一般dubbo默认30s)，一旦超时自动释放资源，因释放资源较快一定程度可抑制资源耗尽问题。但如果在超时释放的时间内陡增大量请求，依然会导致服务宕机不可用。

![screenshots](images/screenshots.gif)

#### 舱壁模式

仓壁模式来源于船舱的设计：船舱都会被隔板分离为多个独立空间，当船体破损时，只会导致部分空间进入，将故障控制在一定范围内，避免整个船体都被淹没。于此类似，我们可以限定每个业务能使用的线程数，避免耗尽整个服务器的资源，因此也叫线程隔离。

![image-20240731190724244](images/image-20240731190724244.png)

#### 限流

限流：限制业务访问的QPS，避免服务因流量的突增而故障。

![image-20240731191206060](images/image-20240731191206060.png)



#### 服务熔断

“熔断器”本身是一种开关装置，当某个服务单元发生故障之后，通过断路器(hystrix)的故障监控，某个异常条件被触发，直接熔断整个服务。向调用方法返回一个符合预期的、可处理的备选响应(FallBack),而不是长时间的等待或者抛出调用方法无法处理的异常，就保证了服务调用方的线程不会被长时间占用，避免故障在分布式系统中蔓延，乃至雪崩。如果目标服务情况好转则恢复调用。服务熔断是解决服务雪崩的重要手段。

如下图：

![image-20240731173233264](images/image-20240731173233264.png)

#### 服务降级

- 当下游的服务因为某种原因**响应过慢**，下游服务主动停掉一些不太重要的业务，释放出服务器资源，增加响应速度！
- 当下游的服务因为某种原因**不可用**，上游主动调用本地的一些降级逻辑，避免卡顿，迅速返回给用户

如下图：

![image-20240731173333312](images/image-20240731173333312.png)

> ### 降级和熔断比较
>
> #### 共同点
>
> - 目的很一致，都是从可用性可靠性着想，为防止系统的整体缓慢甚至崩溃，采用的技术手段；
> - 最终表现类似，对于两者来说，最终让用户体验到的是某些功能暂时不可达或不可用；
> - 粒度一般都是服务级别，当然，业界也有不少更细粒度的做法，比如做到数据持久层（允许查询，不允许增删改）；
> - 自治性要求很高，熔断模式一般都是服务基于策略的自动触发，降级虽说可人工干预，但在微服务架构下，完全靠人显然不可能，开关预置、配置中心都是必要手段；sentinel（阿里巴巴的组件）
>
> #### 异同点
>
> - 触发原因不太一样，服务熔断一般是某个服务（下游服务）故障引起，而服务降级一般是从整体负荷考虑；
> - 管理目标的层次不太一样，熔断其实是一个框架级的处理，每个微服务都需要（无层级之分），而降级一般需要对业务有层级之分（比如降级一般是从最外围服务边缘服务开始）
>
> > 熔断必会触发降级,所以熔断也是降级一种,区别在于熔断是对调用链路的保护,而降级是对系统过载的一种保护处理

##  Polly组件

### 简介

Polly是一个开源的弹性瞬态故障处理库、它可以在程序出现故障、超时，或者返回值达到某种条件的时候进行多种策略处理。比如重试、超时、降级、熔断等等。

Polly可以通过不同策略处理和应对故障场景，主要分为两大类：**被动策略和主动策略**，各自包含如下功能：

#### 被动策略

主要针对故障的处理，避免如下：

- **重试(Retry)**：在实际应用场景中往往有些失败只是瞬时的，经过短暂的延时就可恢复，这种情况就可以采用重试策略；
- **熔断（Circuit Breaker)**：比如在调用接口发生异常时，当多次都返回异常，建议先熔断一段时间，即不再处理业务接口，直接报错；待熔断时间过了之后可以重新处理请求，即快速响应失败比让用户一直等待要合理；
- **回退(Fallback)**：如果失败之后怎么处理？即在发生故障的时候找一个替代逻辑进行处理， 比如返回指定的结果或是进行下一步操作；

#### 主动策略

主要是进行弹性扩展，而不是针对故障处理，关键点是改变原有业务逻辑的执行行为，比如原业务逻辑超时了，就会执行指定的超时处理行为；

- **超时(Timeout )**：确保调用者永远不需要等待超过配置的超时时间，不然就会触发超时异常；主要就是为了提升用户体验；
- **舱壁隔离(Bulkhead Isolation)**：即一个服务的故障不应该影响到整个系统(隔离)；通过控制资源消耗，避免一个故障导致级联服务也故障，最终影响整个系统；目的就是进行并发控制（限流），避免故障带来的大范围影响。
- **缓存(Cache)**：将数据存入缓存中，后续的响应可以从缓存中获取; 目的就是为了提升性能；
- **策略包装( PolicyWrap)**：策略可以组合进行使用；目的就是为了方便各种策略组合进行业务故障处理；

### 超时策略

```c#

                var memberJson = await Policy.TimeoutAsync(5, TimeoutStrategy.Pessimistic, (t, s, y) =>
                {
                    Console.WriteLine("超时了~~~~");
                    return Task.CompletedTask;
                }).ExecuteAsync(async () =>
                {
                    // 业务逻辑
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:5000");
                    var memberResult = await httpClient.GetAsync("/api/polly/timeout");
                    memberResult.EnsureSuccessStatusCode();
                    var json = await memberResult.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    return json;
                });
```

### 重试策略

```c#

                //当发生 HttpRequestException 的时候触发 RetryAsync 重试，并且最多重试3次。
                var memberJson1 = await Policy.Handle<HttpRequestException>().RetryAsync(3).ExecuteAsync(async () =>
                {
                    Console.WriteLine("重试中.....");
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:8000");
                    var memberResult = await httpClient.GetAsync("/member/1001");
                    memberResult.EnsureSuccessStatusCode();
                    var json = await memberResult.Content.ReadAsStringAsync();

                    return json;
                });

                //使用 Polly 在出现当请求结果为 http status_code 500 的时候进行3次重试。
                var memberResult = await Policy.HandleResult<HttpResponseMessage>
                    (x => (int)x.StatusCode == 500).RetryAsync(3).ExecuteAsync(async () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("响应状态码重试中.....");
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:5000");
                    var memberResult = await httpClient.GetAsync("/api/polly/error");

                    return memberResult;
                });
```

### 降级策略

```c#

            //首先我们使用 Policy 的 FallbackAsync("FALLBACK") 方法设置降级的返回值。当我们服务需要降级的时候会返回 "FALLBACK" 的固定值。
            //同时使用 WrapAsync 方法把重试策略包裹起来。这样我们就可以达到当服务调用失败的时候重试3次，如果重试依然失败那么返回值降级为固定的 "FALLBACK" 值。
            var fallback = Policy<string>.Handle<HttpRequestException>().Or<Exception>().FallbackAsync("FALLBACK", (x) =>
            {
                Console.WriteLine($"进行了服务降级 -- {x.Exception.Message}");
                return Task.CompletedTask;
            }).WrapAsync(Policy.Handle<HttpRequestException>().RetryAsync(3));

            var memberJson = await fallback.ExecuteAsync(async () =>
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"http://localhost:5000");
                var result = await httpClient.GetAsync("/api/user/" + 1);
                result.EnsureSuccessStatusCode();
                var json = await result.Content.ReadAsStringAsync();
                return json;

            });
            Console.WriteLine(memberJson);
            if (memberJson != "FALLBACK")
            {
                var member = JsonConvert.DeserializeObject<User>(memberJson);
                Console.WriteLine($"{member!.Id}---{member.Name}");
            }
```

### 熔断策略

### 策略包裹

多种策略进行组装

```c#
            //定义熔断策略
            var circuitBreaker = Policy.Handle<Exception>().CircuitBreakerAsync(
               exceptionsAllowedBeforeBreaking: 2, // 出现几次异常就熔断
               durationOfBreak: TimeSpan.FromSeconds(10), // 熔断10秒
               onBreak: (ex, ts) =>
               {
                   Console.WriteLine("circuitBreaker onBreak ."); // 打开断路器
               },
               onReset: () =>
               {
                   Console.WriteLine("circuitBreaker onReset "); // 关闭断路器
               },
               onHalfOpen: () =>
               {
                   Console.WriteLine("circuitBreaker onHalfOpen"); // 半开
               }
            );

            // 定义重试策略
            var retry = Policy.Handle<HttpRequestException>().RetryAsync(3);
            // 定义降级策略
            var fallbackPolicy = Policy<string>.Handle<HttpRequestException>().Or<BrokenCircuitException>()
                .FallbackAsync("FALLBACK", (x) =>
                {
                    Console.WriteLine($"进行了服务降级 -- {x.Exception.Message}");
                    return Task.CompletedTask;
                })
                .WrapAsync(circuitBreaker.WrapAsync(retry));
            string memberJsonResult = "";

            do
            {
                memberJsonResult = await fallbackPolicy.ExecuteAsync(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri($"http://localhost:5000");
                    var result = await httpClient.GetAsync("/api/user/" + 1);
                    result.EnsureSuccessStatusCode();
                    var json = await result.Content.ReadAsStringAsync();
                    return json;
                });
                Thread.Sleep(1000);
            } while (memberJsonResult == "FALLBACK");

            if (memberJsonResult != "FALLBACK")
            {
                var member = JsonConvert.DeserializeObject<User>(memberJsonResult);
                Console.WriteLine($"{member!.Id}---{member.Name}");
            }
```

当请求出错时-->重试-->两次异常-->打开断路器--->10S后-->半开断路器，再次尝试请求，如果正常则关闭断路器，反之，打开。

## 微服务对接Polly

使用AutoFac AOP注入方式，实现Polly调用

（1）Nuget包引入

	- Autofac
	- Autofac.Extras.DynamicProxy
	- Autofac.Extensions.DependencyInjection

（2）创建Polly策略特性配置类，用于设计策略参数

```C#

    /// <summary>
    /// Polly策略特性配置类（用于设计策略参数）
    /// </summary>
    public class PollyPolicyConfigAttribute : Attribute
    {
        /// <summary>
        /// 最多重试几次，如果为0则不重试
        /// </summary>
        public int MaxRetryTimes { get; set; } = 0;

        /// <summary>
        /// 重试间隔的毫秒数
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 100;

        /// <summary>
        /// 是否启用熔断
        /// </summary>
        public bool IsEnableCircuitBreaker { get; set; } = false;

        /// <summary>
        /// 熔断前出现允许错误几次
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;

        /// <summary>
        /// 熔断多长时间（毫秒）
        /// </summary>
        public int MillisecondsOfBreak { get; set; } = 1000;

        /// <summary>
        /// 执行超过多少毫秒则认为超时（0表示不检测超时）
        /// </summary>
        public int TimeOutMilliseconds { get; set; } = 0;

        /// <summary>
        /// 缓存多少毫秒（0表示不缓存），用“类名+方法名+所有参数ToString拼接”做缓存Key
        /// </summary>

        public int CacheTTLMilliseconds { get; set; } = 0;

        /// <summary>
        /// 回退方法
        /// </summary>
        public string? FallBackMethod { get; set; }
    }
```

（3）创建PollyPolicyAttribute属性（即拦截器），用于AOP注入

```C#

    /// <summary>
    /// 定义AOP特性类及封装Polly策略
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PollyPolicyAttribute : Attribute, IInterceptor
    {
        private static ConcurrentDictionary<MethodInfo, AsyncPolicy> policies
            = new ConcurrentDictionary<MethodInfo, AsyncPolicy>();

        //private static readonly IMemoryCache memoryCache
        //    = new MemoryCache(new MemoryCacheOptions());

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.Method.IsDefined(typeof(PollyPolicyConfigAttribute), true))
            {
                // 直接调用方法本身
                invocation.Proceed();
            }
            else
            {
                PollyPolicyConfigAttribute pollyPolicyConfigAttribute = invocation.Method.GetCustomAttribute<PollyPolicyConfigAttribute>()!;
                //一个PollyPolicyAttribute中保持一个policy对象即可
                //其实主要是CircuitBreaker要求对于同一段代码要共享一个policy对象
                //根据反射原理，同一个方法的MethodInfo是同一个对象，但是对象上取出来的PollyPolicyAttribute
                //每次获取的都是不同的对象，因此以MethodInfo为Key保存到policies中，确保一个方法对应一个policy实例
                policies.TryGetValue(invocation.Method, out AsyncPolicy? policy);
                //把本地调用的AspectContext传递给Polly，主要给FallbackAsync中使用
                // 创建Polly上下文对象(字典)
                Context pollyCtx= new Context();
                pollyCtx["invocation"] = invocation;

                lock (policies)//因为Invoke可能是并发调用，因此要确保policies赋值的线程安全
                {
                    if (policy == null)
                    {
                        policy = Policy.NoOpAsync();//创建一个空的Policy
                        if (pollyPolicyConfigAttribute.IsEnableCircuitBreaker)
                        {
                            policy = policy.WrapAsync(Policy.Handle<Exception>()
                                .CircuitBreakerAsync(pollyPolicyConfigAttribute.ExceptionsAllowedBeforeBreaking,
                                TimeSpan.FromMilliseconds(pollyPolicyConfigAttribute.MillisecondsOfBreak),
                                onBreak: (ex, ts) =>
                                {
                                    Console.WriteLine($"熔断器打开 熔断{pollyPolicyConfigAttribute.MillisecondsOfBreak / 1000}s.");
                                },
                                onReset: () =>
                                {
                                    Console.WriteLine("熔断器关闭，流量正常通行");
                                },
                                onHalfOpen: () =>
                                {
                                    Console.WriteLine("熔断时间到，熔断器半开，放开部分流量进入");
                                }));
                        }
                        if (pollyPolicyConfigAttribute.TimeOutMilliseconds > 0)
                        {
                            policy = policy.WrapAsync(Policy.TimeoutAsync(() =>
                                TimeSpan.FromMilliseconds(pollyPolicyConfigAttribute.TimeOutMilliseconds),
                                Polly.Timeout.TimeoutStrategy.Pessimistic));
                        }
                        if (pollyPolicyConfigAttribute.MaxRetryTimes > 0)
                        {
                            policy = policy.WrapAsync(Policy.Handle<Exception>()
                                .WaitAndRetryAsync(pollyPolicyConfigAttribute.MaxRetryTimes, i =>
                                TimeSpan.FromMilliseconds(pollyPolicyConfigAttribute.RetryIntervalMilliseconds)));
                        }
                        // 定义降级测试
                        var policyFallBack = Policy.Handle<Exception>().FallbackAsync((fallbackContent, token) =>
                        {
                            // 必须从Polly的Context种获取IInvocation对象
                            IInvocation iv = (IInvocation)fallbackContent["invocation"];
                            var fallBackMethod = iv.TargetType.GetMethod(pollyPolicyConfigAttribute.FallBackMethod!);
                            var fallBackResult = fallBackMethod!.Invoke(iv.InvocationTarget, iv.Arguments);
                            iv.ReturnValue = fallBackResult;
                            return Task.CompletedTask;
                        }, (ex, t) =>
                        {
                            Console.WriteLine("====================>触发服务降级");
                            return Task.CompletedTask;
                        });

                        policy = policyFallBack.WrapAsync(policy);
                        //放入到缓存
                        policies.TryAdd(invocation.Method, policy);
                    }
                }

                // 是否启用缓存
                if (pollyPolicyConfigAttribute.CacheTTLMilliseconds > 0)
                {
                    //用类名+方法名+参数的下划线连接起来作为缓存key
                    string cacheKey = "PollyMethodCacheManager_Key_" + invocation.Method.DeclaringType
                                                                       + "." + invocation.Method + string.Join("_", invocation.Arguments);
                    //尝试去缓存中获取。如果找到了，则直接用缓存中的值做返回值
                    //if (memoryCache.TryGetValue(cacheKey, out var cacheValue))
                    //{
                    //    invocation.ReturnValue = cacheValue;
                    //}
                    //else
                    {
                        //如果缓存中没有，则执行实际被拦截的方法
                        Task task = policy.ExecuteAsync(
                            async (context) =>
                            {
                                invocation.Proceed();
                                await Task.CompletedTask;
                            },
                            pollyCtx
                        );
                        task.Wait();

                        ////存入缓存中
                        //using var cacheEntry = memoryCache.CreateEntry(cacheKey);
                        //{
                        //    cacheEntry.Value = invocation.ReturnValue;
                        //    cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(pollyPolicyConfigAttribute.CacheTTLMilliseconds);
                        //}
                    }
                }
                else//如果没有启用缓存，就直接执行业务方法
                {
                    Task task = policy.ExecuteAsync(
                            async (context) =>
                            {
                                invocation.Proceed();
                                await Task.CompletedTask;
                            },
                            pollyCtx
                        );
                    task.Wait();
                }
            }
        }

    }
```

（4）AutoFac IOC注册(Program.cs)

```C#
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
        #region IOC容器
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, buider) =>
                {
                    // 必须使用单例注册
                    buider.RegisterType<UserService>()
                    .As<IUserService>().SingleInstance().EnableInterfaceInterceptors();
                    buider.RegisterType<PollyPolicyAttribute>();

                });
        #endregion
```

(5)在服务接口上添加[Intercept(typeof(Class))]标签

```C#
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IUserService
    {
        User FindUser(int id);

        IEnumerable<User> UserAll();

        #region Polly
        [PollyPolicy]
        [PollyPolicyConfig(FallBackMethod = "UserServiceFallback",
            IsEnableCircuitBreaker = true,
            ExceptionsAllowedBeforeBreaking = 3,
            MillisecondsOfBreak = 1000 * 5,
            CacheTTLMilliseconds = 1000 * 20)]
        User AOPGetById(int id);

        Task<User> GetById(int id);
        #endregion

    }
```



















































