using Newtonsoft.Json;
using System.Text;
using YP.Util.Sockets;

namespace ServerTest
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


            ////创建客户端对象，默认连接本机127.0.0.1,端口为12345
            //SocketServer server = new SocketServer(12345);

            ////绑定当收到服务器发送的消息后的处理事件
            //server.HandleRecMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            //{
            //    zRecvCoun++;
            //    string msg = Encoding.UTF8.GetString(bytes);
            //    if (msg == l2)
            //    {
            //        server.Send(z3);
            //    }
            //    if (msg == l4)
            //    {
            //        server.Send(z5);
            //    }
            //});

            ////绑定向服务器发送消息后的处理事件
            //server.HandleSendMsg = new Action<byte[], SocketServer>((bytes, theClient) =>
            //{
            //    string msg = Encoding.UTF8.GetString(bytes);
            //    Console.WriteLine($"向服务器发送消息:{msg}");
            //});

            ////开始运行客户端
            //server.StartServer();

            //for (int i = 0; i < total; i++)
            //{
            //    client.StartListen(z0);
            //}
            //client.Close();
            //Console.WriteLine("李大爷听到:" + lRecvCount);
            //Console.ReadKey();




            //创建客户端对象，默认连接本机127.0.0.1,端口为12345
            SocketServer server = new SocketServer(12345);

            //处理从客户端收到的消息
            server.HandleRecMsg = new Action<byte[], SocketConnection, SocketServer>((bytes, client, theServer) =>
            {
                lRecvCount++;
                string msg = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"收到消息:{msg}");
                RequestResponse? rr = JsonConvert.DeserializeObject<RequestResponse>(msg);
                if (rr.Payload == z0)
                {                             
                    client.Send(JsonConvert.SerializeObject(new RequestResponse(rr.Serial, l1)));
                    Thread.Sleep(1000);
                    client.Send(JsonConvert.SerializeObject(new RequestResponse(rr.Serial, l2)));
                    Thread.Sleep(1000);
                }
            });

            //处理服务器启动后事件
            server.HandleServerStarted = new Action<SocketServer>(theServer =>
            {
                Console.WriteLine("服务已启动************");
            });

            //处理新的客户端连接后的事件
            server.HandleNewClientConnected = new Action<SocketServer, SocketConnection>((theServer, theCon) =>
            {
                Console.WriteLine($@"一个新的客户端接入，当前连接数：{theServer.GetConnectionCount()}");
            });

            //处理客户端连接关闭后的事件
            server.HandleClientClose = new Action<SocketConnection, SocketServer>((theCon, theServer) =>
            {
                Console.WriteLine($@"一个客户端关闭，当前连接数为：{theServer.GetConnectionCount()}");
            });

            //处理异常
            server.HandleException = new Action<Exception>(ex =>
            {
                Console.WriteLine(ex.Message);
            });

            //服务器启动
            server.StartServer();

            while (true)
            {
                Console.WriteLine("输入:quit，关闭服务器");
                string op = Console.ReadLine();
                if (op == "quit")
                    break;
            }
        }
    }
}