using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisServiceStack
{
	public static class HashTool
	{
		public static void StoreAsHash<T>(T model) where T : class, new()
		{
			//获取当前类型的所有字段
			Type type = model.GetType();
			var fields = type.GetProperties();
			// urn: 类名: id的值
			var hashid = type.FullName;
			Dictionary<string, string> pairs = new Dictionary<string, string>();
			var IdValue = string.Empty;
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].Name.ToLower() == "id")
				{
					//如果你真的把两个相同的id的对象存进去，我可能只改其中一个
					//不可能，如果有两个相同的id的对象存进去，则后面的会把前面的替换掉
					IdValue = fields[i].GetValue(model).ToString();
				}
				else
				{
					// 获取字段的值
					pairs.Add(fields[i].Name, fields[i].GetValue(model).ToString());
				}

			}
			if (IdValue == string.Empty)
			{
				IdValue = DateTime.Now.ToString("yyyyMMdd");
			}
			RedisClient client = new RedisClient("127.0.0.1");
			client.SetRangeInHash(hashid + IdValue, pairs);
		}

		public static T GetFromHash<T>(object id) where T : class, new()
		{
			//获取当前类型的所有字段
			Type type = typeof(T);
			// urn: 类名: id的值
			var hashid = type.FullName;
			RedisClient client = new RedisClient("127.0.0.1");
			var dics = client.GetAllEntriesFromHash(hashid + id.ToString());
			if (dics.Count == 0)
			{
				return new T();
			}
			else
			{
				var model = Activator.CreateInstance(type);
				var fields = type.GetProperties();
				foreach (var item in fields)
				{
					if (item.Name.ToLower() == "id")
					{
						item.SetValue(model, id);
					}
					if (dics.ContainsKey(item.Name))
					{
						item.SetValue(model, dics[item.Name]);
					}
				}
				return (T)model;
			}


		}
	}
}