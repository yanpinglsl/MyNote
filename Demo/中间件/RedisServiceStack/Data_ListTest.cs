using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisServiceStack
{
	class Data_ListTest
	{
		public static void Show()
		{
			using (RedisClient client = new RedisClient("127.0.0.1", 6379))
			{   //删除当前数据库中的所有Key  默认删除的是db0
				client.FlushDb();
				//删除所有数据库中的key 
				client.FlushAll();
				string listid = "clay_list";

				#region 顺序添加

				var caocao = new UserInfo() { Id = 1, Name = "李太白" };
				client.AddItemToList(listid, JsonConvert.SerializeObject(caocao));
				var jiaxu = new UserInfo() { Id = 2, Name = "贾诩" };
				client.AddItemToList(listid, JsonConvert.SerializeObject(jiaxu));
				#endregion

				#region 从左侧向list中添加值 追加
				//var liubei = new UserInfo() { Id = 1, Name = "刘备" };
				//client.PushItemToList(listid, JsonConvert.SerializeObject(liubei));
				#endregion

				#region 从左侧向list中添加值，并设置过期时间
				//var caocao = new UserInfo() { Id = 1, Name = "李太白" };
				//client.AddItemToList(listid, JsonConvert.SerializeObject(caocao));
				//var liubei = new UserInfo() { Id = 2, Name = "刘备" };
				//client.PushItemToList(listid, JsonConvert.SerializeObject(liubei));
				////只在内存中存储一秒
				//client.ExpireEntryAt(listid, DateTime.Now.AddSeconds(1));
				//Console.WriteLine(client.GetListCount(listid));
				//Task.Delay(1 * 1000).Wait();
				//Console.WriteLine("1秒之后");
				//Console.WriteLine(client.GetListCount(listid));
				//雪崩 问题：瞬间大量的数据消失-》大量的数据不要一下的全部消失
				#endregion

				#region 从右侧向list中添加值，并设置过期时间 插队
				//var caocao = new UserInfo() { Id = 1, Name = "李太白" };
				//client.AddItemToList(listid, JsonConvert.SerializeObject(caocao));
				//var jiaxu = new UserInfo() { Id = 2, Name = "贾诩" };

				//client.AddItemToList(listid, JsonConvert.SerializeObject(jiaxu));
				//var gaunyu = new UserInfo() { Id = 3, Name = "关羽" };
				////向右追加就是插队
				//client.PrependItemToList(listid, JsonConvert.SerializeObject(gaunyu));
				//Console.WriteLine("ok");
				//var caomegndeng = new UserInfo() { Id = 3, Name = "曹孟德" };
				//client.PrependItemToList(listid, JsonConvert.SerializeObject(caomegndeng));
				//client.ExpireEntryAt(listid, DateTime.Now.AddSeconds(1));
				//Console.WriteLine(client.GetListCount(listid));
				//Task.Delay(1 * 1000).Wait();
				//Console.WriteLine("1秒之后");
				//Console.WriteLine(client.GetListCount(listid));
				#endregion

				#region  为key添加多个值
				//client.AddRangeToList(listid, new List<string>() { "001", "002", "003", "004" });
				////批量去读取list中的元素
				//var lists = client.GetAllItemsFromList(listid);
				//foreach (var item in lists)
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region 获取key中下标为star到end的值集合
				//client.AddRangeToList(listid, new List<string>() { "001", "002", "003", "004" });
				//var lists = client.GetRangeFromList(listid, 0, 1);//从下标0到1的值
				//foreach (var item in lists)
				//{
				//	Console.WriteLine(item);
				//}
				#endregion

				#region  list 队列和集合操作 mq  
				//var caocao = new UserInfo() { Id = 1, Name = "李太白" };
				//client.AddItemToList(listid, JsonConvert.SerializeObject(caocao));
				//var jiaxu = new UserInfo() { Id = 2, Name = "贾诩" };
				//client.AddItemToList(listid, JsonConvert.SerializeObject(jiaxu));
				//var gaunyu = new UserInfo() { Id = 3, Name = "关羽" };
				//client.AddItemToList(listid, JsonConvert.SerializeObject(gaunyu));
				////移除尾部 并返回移除的数据 先删再给数据
				//Console.WriteLine(client.RemoveEndFromList(listid));
				//foreach (var item in client.GetAllItemsFromList(listid))
				//{
				//	Console.WriteLine(JsonConvert.DeserializeObject<UserInfo>(item).Name);
				//}
				//移除头部并返回移除的数据
				//Console.WriteLine(client.RemoveStartFromList(listid)); 
				//foreach (var item in client.GetAllItemsFromList(listid))
				//{
				//	Console.WriteLine(JsonConvert.DeserializeObject<UserInfo>(item).Name);
				//}

				//从一个list的尾部移除一个数据，添加到另外一个list的头部，并返回移动的值
				//Console.WriteLine(client.PopAndPushItemBetweenLists(listid, "newlist"));
				//Console.WriteLine("移动之后新队列的元素结果");
				//Console.WriteLine(client.GetItemFromList("newlist",0));
				//根据下标获取想要的集合元素，不做移除操作
				//var userstr = client.GetItemFromList(listid, 0);
				//Console.WriteLine(JsonConvert.DeserializeObject<UserInfo>(userstr).Name);
				////修改当前下标的结果
				//client.SetItemInList(listid, 0, "new value");

				#endregion

			}
		}
	}
}
