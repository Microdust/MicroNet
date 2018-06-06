using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            LocalHost host = new LocalHost(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = false,
                MaxConnections = 5,
                Name = "Client",
                Timeout = 1,
                NetworkRate = 20
            });

            host.Start();


            while (isPooling)
            {
                host.Tick();
                Thread.Sleep(50);
            }

        
        }


        private static void Server()
        {

            LocalHost host = new LocalHost(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "Server",
                Timeout = 1,
                NetworkRate = 20
            });


            host.Start();

            while (isPooling)
            {
                host.Tick();
                Thread.Sleep(50);
            }
        }


        public static void Main(string[] args)
        {   
         
            Thread[] clients = new Thread[1];
            var server = new Thread(Server);
            server.Start();
           /* Thread.Sleep(100);

            for (int i = 0; i < clients.Length; i++)
            {
                clients[i] = new Thread(Client);
                clients[i].Start();
            }
            */
            server.Join();    

        }



    }
}
