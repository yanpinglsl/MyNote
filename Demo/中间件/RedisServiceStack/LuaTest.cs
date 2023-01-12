using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisServiceStack
{
	public class LuaTest
	{
		public static void Show()
		{

			using (var client = new RedisClient("192.168.1.211", 6379))
			{

				//Console.WriteLine(client.ExecLuaAsString(@"return  redis.call('get','name')"));

				//库存
				//Console.WriteLine(client.ExecLuaAsString(@"redis.call('set','number','10')"));
				var lua = @"local count = redis.call('get',KEYS[1])
				                        if(tonumber(count)>=0)
				                        then
				                            redis.call('INCR',ARGV[1])
				                            return redis.call('DECR',KEYS[1])
				                        else
				                            return -1
				                        end";
				Console.WriteLine(client.ExecLuaAsString(lua, keys: new[] { "number" }, args: new[] { "ordercount" }));


			}
		}
	}
}
