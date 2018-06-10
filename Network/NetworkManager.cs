﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{

    public unsafe abstract partial class NetworkManager
    {
        public NetConfiguration config;

        private Thread networkThread;
        private bool isRunning;
        private ENet.Host* ENetHost;

        private ENet.Peer*[] connections;

        private IncomingMessage msg;

      
        public NetworkManager(NetConfiguration configuration)
        {
            this.config = configuration;            

            networkThread = new Thread(new ThreadStart(Service))
            {
                Name = config.Name,                
                IsBackground = true
            };
            
        }

        public void Start()
        {
            networkThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        private void Initialize()
        {
            ENet.Initialize();

            InitializePools(config.MessagePoolSize);
            InitializeQueues(config.MessageBufferSize);
            MessagePool.InitializePool(config.MessagePoolSize, config.MessageBufferSize);

            connections = new ENet.Peer*[config.MaxConnections];

            if (config.AllowConnectors)
            {
                ENet.Address address = new ENet.Address();
                address.Port = config.Port;


                ENet.AddressSetHost(ref address, config.LocalAddress);
                ENetHost = ENet.CreateHost(ref address, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth, config.AppIdentification);
            }
            else
            {
                ENetHost = ENet.CreateHost(null, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth, config.AppIdentification);

            }

            if (ENetHost == null)
            {
                Debug.Error(config.Name, ": Failed to create host");
                return;
            }



            IncomingMessage readyEvent = GetIncomingMessage();
            readyEvent.Event = EventMessage.NetworkReady;
            IncomingEnqueue(readyEvent);

            isRunning = true;
        }

        /// <summary>
        /// Request to connect to a remote host at specified address and port.
        /// </summary>
        public void Connect(string address, ushort port)
        {
            ENet.Address remoteAddr = new ENet.Address();
            remoteAddr.Port = port;
            ENet.AddressSetHost(ref remoteAddr, Encoding.ASCII.GetBytes(address));


            ENet.Connect(ENetHost, ref remoteAddr, (IntPtr)config.DefaultChannelAmount);
        }


        /// <summary>
        /// Request to connect to a remote host at specified address and port.
        /// </summary>
        public void Connect(System.Net.IPAddress address, ushort port)
        {
            ENet.Address remoteAddr = new ENet.Address();
            remoteAddr.Port = port;
            ENet.AddressSetHost(ref remoteAddr, address.GetAddressBytes());

            ENet.Connect(ENetHost, ref remoteAddr, (IntPtr)config.DefaultChannelAmount);

        }


    

        /// <summary>
        /// Queues a packet to be sent to all peers associated with the host.
        /// </summary>
        public void Broadcast(OutgoingMessage msg, byte channelId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroBroadcast(ENetHost, channelId, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Queues a packet to be sent to all peers associated with the host. Default channel = 0
        /// </summary>
        public void Broadcast(OutgoingMessage msg)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroBroadcast(ENetHost, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void Send(OutgoingMessage msg, uint connectionId, byte channelId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(connections[connectionId], channelId, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection. Default channel = 0
        /// </summary>
        public void Send(OutgoingMessage msg, uint connectionId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(connections[connectionId], 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }


        internal int SocketSend(IPEndPoint addr, OutgoingMessage msg)
        {
            ENet.Address address = new ENet.Address();
            address.Port = (ushort)addr.Port;

            if (ENet.AddressSetHost(ref address, addr.Address.GetAddressBytes()) != 0)
            {
                Debug.Log(config.Name, "Failed to resolve host name");
            }

            Debug.Log(config.Name, " hostname: ", address.Host.ToString(), " : ", address.Port.ToString());
            
            fixed (byte* bytes = msg.Data)
            {
               return ENet.MicroSocketSend(ENetHost, ref address, bytes, (IntPtr)msg.ByteCount);
            }
        }

        internal void NATPunching(IPEndPoint addr)
        {
            ENet.Address address = new ENet.Address();
            address.Port = (ushort)addr.Port;

            if (ENet.AddressSetHost(ref address, addr.Address.GetAddressBytes()) != 0)
            {
                Debug.Log(config.Name, "Failed to resolve host name");
            }

            Debug.Log(config.Name, " hostname: ", address.Host.ToString(), " : ", address.Port.ToString());

            fixed (byte* bytes = msg.Data)
            {
                for (int i = 0; i < 8; i++)
                {
                    ENet.MicroSocketSend(ENetHost, ref address, bytes, (IntPtr)msg.ByteCount);
                    Thread.Sleep(5);
                }
            }

            if (!config.AllowConnectors)
            {
                ENet.Connect(ENetHost, ref address, (IntPtr)config.DefaultChannelAmount);
            }
            else
            {
                fixed (byte* bytes = msg.Data)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        ENet.MicroSocketSend(ENetHost, ref address, bytes, (IntPtr)msg.ByteCount);
                        Thread.Sleep(5);
                    }
                }
            }

        }

        public void Disconnect(uint connectionId, uint flag)
        {
            if (connectionId > connections.Length)
            {
                Debug.Error(config.Name, ": Disconnect Id: ", connectionId.ToString(), " -- Out of connection array bounds");
                return;
            }

            ENet.DisconnectPeer(connections[connectionId], flag);
        }

        public void Disconnect(uint connectionId)
        {
            if (connectionId > connections.Length)
            {
                Debug.Error(config.Name, ": Disconnect Id: ", connectionId.ToString(), " -- Out of connection array bounds");
                return;
            }

            ENet.DisconnectPeer(connections[connectionId], 0);
        }

        public abstract void OnConnect(RemoteConnection remote);
        public abstract void OnDisconnect(RemoteConnection remote);
        public abstract void OnReceived(IncomingMessage msg);
        public abstract void OnReady();
        public abstract void OnStop();

        public IncomingMessage ReadMessage()
        {
            return IncomingDequeue();
        }

        public void Tick()
        {
            if ((msg = IncomingDequeue()) == null)
                return;

            switch (msg.Event)
            {
                case EventMessage.Connect:
                OnConnect(msg.Remote);
                break;

                case EventMessage.Disconnect:
                OnDisconnect(msg.Remote);
                break;

                case EventMessage.Receive:
                OnReceived(msg);
                break;

                case EventMessage.NetworkReady:
                OnReady();
                break;

                case EventMessage.NetworkStopped:
                OnStop();
                break;
            }

            Recycle(msg);

        }

        private void Service()
        {
            int sleepTime = 1000 / config.NetworkRate;
            uint serviceWait = config.Timeout;
            int connectionCount = 0;
            Initialize();
            ENet.Event evt;
            IncomingMessage internalMsg;

            
        
            while (isRunning)
            {

                if (ENet.Service(ENetHost, out evt, serviceWait) > 0)
                {
                    switch (evt.type)
                    {
                        case EventMessage.Connect:
                        {
                            connections[evt.peer->incomingPeerID] = evt.peer;

                            internalMsg = GetIncomingMessage();
                            
                            internalMsg.Event = evt.type;
                            internalMsg.Remote.Peer = evt.peer;

                            IncomingEnqueue(internalMsg);

                            connectionCount++;
                            break;
                        }
                        case EventMessage.Disconnect:
                        {
                            connections[evt.peer->incomingPeerID] = evt.peer;

                            internalMsg = GetIncomingMessage();

                            internalMsg.Type = evt.data;

                            internalMsg.Event = evt.type;
                            internalMsg.Remote.Peer = evt.peer;

                            IncomingEnqueue(internalMsg);

                            connectionCount--;
                            break;
                        }
                        case EventMessage.Receive:
                        {
                            internalMsg = GetIncomingMessage();

                            internalMsg.Type = evt.data;
                            internalMsg.Event = evt.type;
                            internalMsg.Remote.Peer = evt.peer;

                            int length = (int)evt.packet->dataLength;

                            if (length > internalMsg.Data.Length)
                            {
                                Debug.Log("Incoming Message array was too big, had to resize");
                                internalMsg.Data = new byte[length];
                            }

                            Marshal.Copy(evt.packet->data, internalMsg.Data, 0, length);

                            IncomingEnqueue(internalMsg);
                            break;
                        }

                    }
                
                }
                Thread.Sleep(sleepTime);
            }

            ENet.DestroyHost(ENetHost);
            connections = null;
            ENetHost = null;

            IncomingMessage stoppedEvent = GetIncomingMessage();
            stoppedEvent.Event = EventMessage.NetworkStopped;

            IncomingEnqueue(stoppedEvent);

        }   

    }


}
