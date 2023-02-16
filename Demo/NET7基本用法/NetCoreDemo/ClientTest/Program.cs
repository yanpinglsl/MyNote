using Newtonsoft.Json;
using System.Text;
using YP.Util.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientTest
{

    record RequestResponse(int Serial, string Payload);
    internal class Program
    {
        static void Main(string[] args)
        {
            int zRecvCoun = 0; // 张大爷听到了多少句话
            int lRecvCount = 0; // 李大爷听到了多少句话
            int total = 100000; // 总共需要遇见多少次

            string z0 = "吃了没，您吶?";
            string z3 = "嗨！吃饱了溜溜弯儿。";
            string z5 = "回头去给老太太请安！";
            string l1 = "刚吃。";
            string l2 = "您这，嘛去？";
            string l4 = "有空家里坐坐啊。";


            //创建客户端对象，默认连接本机127.0.0.1,端口为12345
            SocketClient client = new SocketClient(12345);

            //绑定当收到服务器发送的消息后的处理事件
            client.HandleRecMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                zRecvCoun++;
                string msg = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"收到消息:{msg}");
                RequestResponse? rr = JsonConvert.DeserializeObject<RequestResponse>(msg);
                if (rr.Payload == l2)
                {
                    client.Send(JsonConvert.SerializeObject(new RequestResponse(rr.Serial, z3)));
                }
                if (rr.Payload == l4)
                {
                    client.Send(JsonConvert.SerializeObject(new RequestResponse(rr.Serial, z5)));
                }
                Thread.Sleep(1000);
            });

            //绑定向服务器发送消息后的处理事件
            client.HandleSendMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                string msg = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"向服务器发送消息:{msg}");
            });

            //开始运行客户端
            client.StartClient();

            for (int i = 0; i < total; i++)
            {
                string json = JsonConvert.SerializeObject(new RequestResponse(i, z0));
                client.Send(json);
                Thread.Sleep(1000);
            }
            client.Close();
            Console.WriteLine("张大爷听到:" + zRecvCoun);
            Console.ReadKey();
        }
    }
}