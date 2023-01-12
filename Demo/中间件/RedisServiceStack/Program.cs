
using RedisServiceStack;

try
{
	//using (RedisClient client = new RedisClient("192.168.1.211", 6379))
	//{
	//	client.Custom("bf.add", "myfilter", "dddd");
	//	Console.WriteLine(client.Custom("bf.exists", "myfilter", "dddd4444").Text);
	//}
	Console.WriteLine("ok");
	#region String
	{
		//Console.WriteLine("*****************String****************");
		//Data_StringTest.Show();
	}
	#endregion

	#region Hash
	{
		//Console.WriteLine("*****************Hash****************");
		//Data_HashTest.Show();
	}
	#endregion

	#region Set ZSet
	{
		//Console.WriteLine("*****************Set ZSet****************");
		//Data_SetAndZsetTest.Show();
	}
	#endregion
	#region List
	{
		//Console.WriteLine("*****************List****************");
		//Data_ListTest.Show();
	}
	#endregion

	#region 事务
	{
		//Console.WriteLine("*****************Lua****************");
		//TransAction.Show();
	}
	#endregion
	#region Lua
	{
		Console.WriteLine("*****************Lua****************");
		LuaTest.Show();
	}
	#endregion

}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}
Console.ReadLine();
