using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
                NetworkRate = 50
            });

            host.Start();


            while (isPooling)
            {
                host.Tick();
                Thread.Sleep(50);
            }

        
        }


        private static void NATServer()
        {
            NATServer host = new NATServer(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "NATServer",
                Timeout = 1,
                NetworkRate = 50
            }, 1024);

            host.Start();


            while (isPooling)
            {
                host.Tick();
                Thread.Sleep(50);
            }

        }


        private static void NATClient()
        {
            NATPeer host = new NATPeer(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = false,
                MaxConnections = 5,
                Name = "NATClient",
                Timeout = 1,
                NetworkRate = 50
            });

            host.Start();


            while (isPooling)
            {
                host.Tick();
                Thread.Sleep(50);
            }

        }

        private static void NATHost()
        {
            NATPeer host = new NATPeer(new NetConfiguration(5001)
            {
                Port = 8080,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "NATHost",
                Timeout = 1,
                NetworkRate = 50
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
                NetworkRate = 500
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

            Thread[] clients = new Thread[2];
            var server = new Thread(NATServer);
            server.Start();
            Thread.Sleep(100);

            clients[1] = new Thread(NATHost);
            clients[1].Start();

            clients[0] = new Thread(NATClient);
            clients[0].Start();



            /*
            for (int i = 0; i < clients.Length; i++)
            {
                clients[i] = new Thread(NATClient);
                clients[i].Start();
            }
            */

            server.Join(); 

        }



    }
}
