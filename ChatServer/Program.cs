using System.Net.Sockets;
using System.Net;
class server_config
{
    public string server_ip{get; set;}
    public string server_port{get; set;} 
    public string eula{get; set;}  
}

class Program
{   
    static List<ForClient> clientlist = new List<ForClient>();
    static void Main(string[] args)
    {  
        try
        {
            StreamReader r = new StreamReader("config.json");
            string jsonString = r.ReadToEnd();
            server_config m = Newtonsoft.Json.JsonConvert.DeserializeObject<server_config>(jsonString);

            if(m.eula == "false")
            {
                Console.WriteLine("You need to agree our elua in config.json, just change the value to true");
                Thread.Sleep(5000);
            }
            else
            {
                Console.WriteLine(".Net 6.0 supported!");
                Console.WriteLine("Server Listen on {0}:{1}", m.server_ip, m.server_port);

                Socket TcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TcpServer.Bind(new IPEndPoint(IPAddress.Parse(m.server_ip),Convert.ToInt32(m.server_port)));
                TcpServer.Listen(100);
                Console.WriteLine("Server is Started!");
                Console.WriteLine("Waiting for a client...");
            
                while (true)
                {
                    Socket clientSocket = TcpServer.Accept();
                    Console.WriteLine("[IP: {0}] Client Connected!", clientSocket.RemoteEndPoint);
                    ForClient cc = new ForClient(clientSocket);//每个客户端通信逻辑放到client
                    clientlist.Add(cc);
                }
            } 
        }

        catch(System.IO.FileNotFoundException)
        {
            Console.WriteLine("config.json not found, creating default config.json");
            server_config m = new server_config();
            m.server_ip = "0.0.0.0";
            m.server_port = "2000";
            m.eula = "false";
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(m);
            StreamWriter w = new StreamWriter("config.json");
            w.Write(jsonString);
            w.Close();
            Console.WriteLine("config.json created, please edit it and restart the server");
            Thread.Sleep(5000);
            return;
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
