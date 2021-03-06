﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroNet.Network;
using MicroNet.Network.NAT;

namespace MicroNet
{
    public unsafe class Program
    {
        private static byte[] remoteNin = { 80, 60, 202, 28 };
        private static byte[] localHost = { 127, 0, 0, 1 };
        private static byte[] myHost = { 89, 233, 23,45 };
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
                Port = 5000,
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
                Port = 5000,
                AllowConnectors = false,
                MaxConnections = 5,
                Name = "NATClient",
                Timeout = 1,
                NetworkRate = 50,
                ServerEndPoint = new IPEndPoint(new IPAddress(localHost), 5000)
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
                Port = 5555,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "NATHost",
                Timeout = 1,
                NetworkRate = 50,
                ServerEndPoint = new IPEndPoint(new IPAddress(localHost), 5000)
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
                Port = 5000,
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
            /*
            var server = new Thread(Server);
            server.Start();
            Thread.Sleep(100);
            server.Join();
            */

            /*
               bool isRunning = true;


               while(isRunning)
               {
                   Console.WriteLine("1 for hosting relay\n2 for hosting NAT\n3 for client");
                   ConsoleKeyInfo key = Console.ReadKey();
                   Console.WriteLine();
                   if (key.Key == ConsoleKey.D1)
                   {
                       var server = new Thread(NATServer);
                       server.Start();
                       server.Join();
                   }
                   else if (key.Key == ConsoleKey.D2)
                   {
                       var host = new Thread(NATHost);
                       host.Start();
                       host.Join();
                   }
                   else if (key.Key == ConsoleKey.D3)
                   {
                       var client = new Thread(NATClient);
                       client.Start();
                       client.Join();
                   }
                   else
                   {
                       Console.WriteLine("Wrong key input, please try again...");
                   }
               }
               */
   
      
            var server = new Thread(NATServer);
            server.Start();
            Thread.Sleep(100);


            Thread[] clients = new Thread[2];
            clients[0] = new Thread(NATHost);
            clients[0].Start();

            /*
            Thread.Sleep(250);

            clients[1] = new Thread(NATClient);
            clients[1].Start();

            server.Join();
            */
            



            //            clients[0] = new Thread(NATClient);

            // Console.ReadKey();
            //          clients[0].Start();


            /*
                        for (int i = 0; i < clients.Length; i++)
                        {
                            clients[i] = new Thread(NATClient);
                            clients[i].Start();
                        }



            */

        }



    }
}
