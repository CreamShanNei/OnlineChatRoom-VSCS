using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;

class Program
{
    static List<ForClient> clientlist = new List<ForClient>();
    static void Main(string[] args)
    {
        Console.WriteLine("输入服务端监听IP");
        string ipadd = Console.ReadLine();
        Console.WriteLine("输入服务端监听端口");
        int port = Console.Read();
        Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TcpServer.Bind(new IPEndPoint(IPAddress.Parse(ipadd),port));
        TcpServer.Listen(100);
        Console.WriteLine("服务器开启");
        Console.WriteLine("监听端口：" + ipadd + ":" + port.ToString());

        while (true)
        {
            Socket clientSocket = TcpServer.Accept();
            Console.WriteLine("新接入一个客户端");
            ForClient cc = new ForClient(clientSocket);//每个客户端通信逻辑放到client
            clientlist.Add(cc);
        }

    }

    public static void BroadcastMessage(string message)
    {
        var notConnectedList = new List<ForClient>();

        foreach (var clients in clientlist)
        {
            if (clients.Connected)
                clients.SendMessage(message);
            else
            {
                notConnectedList.Add(clients);
            }
        }
        foreach (var temp in notConnectedList)
        {
            clientlist.Remove(temp);
        }
    }
}
