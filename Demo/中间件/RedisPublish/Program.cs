
using ServiceStack.Redis;

try
{
	//创建一个公众号--创建一个主题
	Console.WriteLine("发布服务");
	IRedisClientsManager redisClientManager = new PooledRedisClientManager("127.0.0.1:6379");
	string topicname = "Send_Log";
	RedisPubSubServer pubSubServer = new RedisPubSubServer(redisClientManager, topicname)
	{
		OnMessage = (channel, msg) =>
		{
			Console.WriteLine($"从频道：{channel}上接受到消息：{msg},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
			Console.WriteLine("___________________________________________________________________");
		},
		OnStart = () =>
		{
			Console.WriteLine("发布服务已启动");
			Console.WriteLine("___________________________________________________________________");
		},
		OnStop = () => { Console.WriteLine("发布服务停止"); },
		OnUnSubscribe = channel => { Console.WriteLine(channel); },
		OnError = e => { Console.WriteLine(e.Message); },
		OnFailover = s => { Console.WriteLine(s); },
	};
	//接收消息
	pubSubServer.Start();
	while (1 == 1)
	{
		Console.WriteLine("请输入记录的日志");
		string message = Console.ReadLine();
		redisClientManager.GetClient().PublishMessage(topicname, message);
	}
}
catch (Exception ex)
{

	Console.WriteLine(ex.Message);
}