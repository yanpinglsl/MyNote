using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisServiceStack
{
	public class TransAction
	{
		public static void Show()
		{
			try
			{

				//事务模式 
				using (RedisClient client = new RedisClient("127.0.0.1", 6379))
				{
					//删除当前数据库中的所有Key  默认删除的是db0
					client.FlushDb();
					//删除所有数据库中的key 
					client.FlushAll();

					client.Set("a", "1");
					client.Set("b", "1");
					client.Set("c", "1");

					////获取当前这三个key的版本号 实现事务
					client.Watch("c");
					using (var trans = client.CreateTransaction())
					{
						trans.QueueCommand(p => p.Set("a", "3"));
						trans.QueueCommand(p => p.Set("b", "3"));
						trans.QueueCommand(p => p.Set("c", "3"));

						var flag = trans.Commit();
						Console.WriteLine(flag);
					}
					//根据key取出值，返回string
					Console.WriteLine(client.Get<string>("a") + ":" + client.Get<string>
					("b") + ":" + client.Get<string>
					("c"));
					Console.ReadLine();
				}


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadLine();
		}
	}
}
