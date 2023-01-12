using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisSubscribeTxt
{
	public class Logger
	{

		/// <summary>
		/// 写入文本日志
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="type"></param>
		/// <param name="content"></param>
		public static void WriteTxtLogs(string fileName, string type, string content)
		{
			string path = AppDomain.CurrentDomain.BaseDirectory;
			if (!string.IsNullOrEmpty(path))
			{
				path = AppDomain.CurrentDomain.BaseDirectory + fileName;
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				path = path + "\\" + DateTime.Now.ToString("yyyyMMdd");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
				if (!File.Exists(path))
				{
					FileStream fs = File.Create(path);
					fs.Close();
				}
				if (File.Exists(path))
				{
					StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
					sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + type + "-->" + content);
					//  sw.WriteLine("----------------------------------------");
					sw.Close();
				}
			}
		}
	}
}
