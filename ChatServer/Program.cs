using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System;

class Program
{
    static List<ForClient> clientlist = new List<ForClient>();
    static void Main(string[] args)
    {

        Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TcpServer.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 2000));
        TcpServer.Listen(100);
        Console.WriteLine("服务器开启");

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
