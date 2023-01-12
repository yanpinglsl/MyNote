using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisServiceStack
{
	class Data_SetAndZsetTest
	{
		public static void Show()
		{
			using (RedisClient client = new RedisClient("127.0.0.1", 6379))
			{   //删除当前数据库中的所有Key  默认删除的是db0
				client.FlushDb();
				//删除所有数据库中的key 
				client.FlushAll();
				#region Set 不重复集合
				string key = "clay_set";
				//投票 
				#region 添加键值 //就是自动去重，再带去重的功能
				var litaibai = new UserInfo() { Id = 1, Name = "李太白" };
				//client.AddItemToList(key, JsonConvert.SerializeObject(litaibai));
				//client.AddItemToList(key, JsonConvert.SerializeObject(litaibai));
				//client.AddItemToSet(key, JsonConvert.SerializeObject(litaibai));
				//client.AddItemToSet(key, JsonConvert.SerializeObject(litaibai));
				//client.AddItemToSet(key, JsonConvert.SerializeObject(litaibai));
				//client.AddItemToSet(key, JsonConvert.SerializeObject(litaibai));
				//Console.WriteLine("***完成了");
				#endregion

				#region 随机获取key集合中的一个值，获取当前setid中的所有值
				//批量的去操作set 集合
				//Console.WriteLine("set 开始了");
				client.AddRangeToSet(key, new List<string>() { "001", "001", "002", "003", "003", "004" });
				//////当前setid中的值数量
				//Console.WriteLine(client.GetSetCount(key));

				////随机获取key集合中的一个值 如果有需要取随机数也可以用
				//Console.WriteLine(client.GetRandomItemFromSet(key));
				////获取当前setid中的所有值
				//var lists = client.GetAllItemsFromSet(key);
				//Console.WriteLine("展示所有的值");
				//foreach (var item in lists)
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 随机删除key集合中的一个值
				//client.AddRangeToSet(key, new List<string>() { "001", "001", "002" });
				//////随机删除key集合中的一个值 返回当前删掉的这个值
				//Console.WriteLine("随机删除的值" + client.PopItemFromSet(key));
				//var lists = client.GetAllItemsFromSet(key);
				//Console.WriteLine("展示删除之后所有的值");
				//foreach (var item in lists)
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 根据小key 删除  一个key 对象多个value
				//client.AddRangeToSet(key, new List<string>() { "001", "001", "002" });
				//client.RemoveItemFromSet(key, "001");
				//var lists = client.GetAllItemsFromSet(key);
				//Console.WriteLine("展示删除之后所有的值");
				//foreach (var item in lists)
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 从fromkey集合中移除值为value的值，并把value添加到tokey集合中
				//client.AddRangeToSet("fromkey", new List<string>() { "003", "001", "002", "004" });
				//client.AddRangeToSet("tokey", new List<string>() { "001", "002" });
				////从fromkey 中把元素004 剪切到tokey 集合中去
				//client.MoveBetweenSets("fromkey", "tokey", "004");
				//Console.WriteLine("fromkey data ~~~~~~");
				//foreach (var item in client.GetAllItemsFromSet("tokey"))
				//{
				//	Console.WriteLine(item);
				//}

				//Console.WriteLine("tokey data ~~~~~~");
				//foreach (var item in client.GetAllItemsFromSet("tokey"))
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 并集  把两个集合合并起来，然后去重

				//client.AddRangeToSet("keyone", new List<string>() { "001", "002", "003", "004" });
				//client.AddRangeToSet("keytwo", new List<string>() { "001", "002", "005" });
				//var unionlist = client.GetUnionFromSets("keyone", "keytwo");
				//Console.WriteLine("返回并集结果");
				//foreach (var item in unionlist)
				//{
				//	Console.WriteLine(item);
				//}
				//把 keyone 和keytwo 并集结果存放到newkey 集合中
				//client.StoreUnionFromSets("newkey", "keyone", "keytwo");
				//Console.WriteLine("返回并集结果的新集合数据");
				//foreach (var item in client.GetAllItemsFromSet("newkey"))
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 交集 获取两个集合中共同存在的元素
				//client.AddRangeToSet("keyone", new List<string>() { "001", "002", "003", "004" });
				//client.AddRangeToSet("keytwo", new List<string>() { "001", "002", "005" });
				//var Intersectlist = client.GetIntersectFromSets("keyone", "keytwo");
				//Console.WriteLine("交集的结果");
				//foreach (var item in Intersectlist)
				//{
				//	Console.WriteLine(item);
				//}
				////把 keyone 和keytwo 交集结果存放到newkey 集合中
				//client.StoreIntersectFromSets("newkey", "keyone", "keytwo");
				//Console.WriteLine("返回交集结果的新集合数据");
				//foreach (var item in client.GetAllItemsFromSet("newkey"))
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#endregion

				#region  sorted set
				//string zsett_key = "clay_zset";
				////添加一个kye 如果有相同的值得花，则会替换（覆盖）进去，不会因为分数保留
				//client.AddItemToSortedSet(zsett_key, "cc", 33);
				//client.AddItemToSortedSet(zsett_key, "cc", 44);
				//client.AddItemToSortedSet(zsett_key, "cc", 22);
				//Console.WriteLine("ok");
				////获取当前value的结果
				//Console.WriteLine(client.GetItemIndexInSortedSet(zsett_key, "cc"));
				////批量操作多个key ，给多个key 赋值1
				//client.AddRangeToSortedSet(zsett_key, new List<string>() { "a", "b" }, 1);

				//foreach (var item in client.GetAllItemsFromSortedSet(zsett_key))
				//{
				//	Console.WriteLine(item);
				//}
				client.AddItemToSortedSet("蜀国", "刘备", 5);
				client.AddItemToSortedSet("蜀国", "关羽", 2);
				client.AddItemToSortedSet("蜀国", "张飞", 3);
				client.AddItemToSortedSet("魏国", "刘备", 5);
				client.AddItemToSortedSet("魏国", "关羽", 2);
				client.AddItemToSortedSet("蜀国", "张飞", 3);
				////获取 key为蜀国的下标0，到2
				IDictionary<String, double> Dic = client.GetRangeWithScoresFromSortedSet("蜀国", 0, 2);
				 
				foreach (var r in Dic)
				{
					Console.WriteLine(r.Key + ":" + r.Value);
				}
				//var DicString = client.StoreIntersectFromSortedSets("2", "蜀国", "魏国");
				//var ss = client.GetAllItemsFromSortedSet("2");
				//foreach (var r in DicString)
				//{
				//	Console.WriteLine(r.Key + ":" + r.Value);   
				//}

			    //int  int32
			 
				#endregion
			}
		}
	}
}
