using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroNet.Network;


namespace MicroNet
{
    public unsafe class Program
    {
        private static bool isPooling = true;

        private static void Client()
        {

            Host host = new Host(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = false,
                MaxConnections = 5,
                Name = "Client",               
            });

            host.Initialize();
            host.Connect("127.0.0.1", 8080);

            IncomingMessage msg;

            while (isPooling)
            {
                while ((msg = host.Service()) != null)
                {
                    Debug.Log("Client received: ", msg.ReadString());
                }

                
            }

        }

        private static void Server()
        {
            Host host = new Host(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "Server",                
            });

            host.Initialize();

            IncomingMessage msg;
            OutgoingMessage outgoing = new OutgoingMessage();
            outgoing.DeliveryMethod = DeliveryMethod.None;
            outgoing.Write("I must have called a thousand times...");        

            while (isPooling)
            {
                while ((msg = host.Service()) != null)
                {
                    Debug.Log("Server received: ", msg.ReadString());
                }
            }
                
    }
            

        public static void Main(string[] args)
        {
            Debug.Log("Initializing ENet...");

            ENet.Initialize();
            

            var server = new Thread(Server);
            server.Start();
            Thread.Sleep(250);
            var client = new Thread(Client);
            client.Start();

            server.Join();
            client.Join();

            Console.ReadKey();
        }

    }
}
