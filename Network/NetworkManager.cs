using MicroNet.Network.NAT;
using System;
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
        private bool isRunning = false;
        private bool isPaused = false;

        private ENet.Host* host;
        private RemoteConnection[] connections;

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

        public NATServerConnection CreateNATServerConnection()
        {
            return new NATServerConnection(host, config.ServerEndPoint);
        }

        public void Start()
        {
            if (networkThread.ThreadState == System.Threading.ThreadState.Unstarted)
            networkThread.Start();
        }

        public void Pause()
        {
            isPaused = true;
            isRunning = false;
        }

        public void Resume()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
            isPaused = false;
        }

        public void ModifyNetworkConfiguration(NetConfiguration config)
        {
            if(isRunning)
            {
                Debug.Error("Attempted to modify internal configuration while network is running. Pause first.");
                return;
            }

            this.config = config;

            Initialize();
        }


        private void Initialize()
        {
            ENet.Initialize();

            InitializePools();
            InitializeQueues(config.IncomingBufferSize);           

            connections = new RemoteConnection[config.MaxConnections];

            if (config.AllowConnectors)
            {
                ENet.Address address = new ENet.Address();
                address.Port = config.Port;


                ENet.AddressSetHost(ref address, config.LocalAddress);
                host = ENet.CreateHost(ref address, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth, config.AppIdentification);
            }
            else
            {
                host = ENet.CreateHost(null, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth, config.AppIdentification);

            }

            if (host == null)
            {
                Debug.Error(config.Name, ": Failed to create host");
                return;
            }

            IncomingMessage readyEvent = GetIncomingMessage();
            readyEvent.Event = EventMessage.NetworkReady;
            IncomingEnqueue(readyEvent);

            isRunning = true;
        }

        #region Connection Methods
        /// <summary>
        /// Request to connect to a remote host at specified address and port.
        /// </summary>
        public void Connect(string address, ushort port)
        {
            ENet.Address remoteAddr = new ENet.Address();
            remoteAddr.Port = port;
            ENet.AddressSetHost(ref remoteAddr, Encoding.ASCII.GetBytes(address));

            ENet.Connect(host, ref remoteAddr, (IntPtr)config.DefaultChannelAmount);
        }


        /// <summary>
        /// Request to connect to a remote host at specified address and port.
        /// </summary>
        public void Connect(IPEndPoint ipEndPoint)
        {
            // Check if this technique actually works in anycase. Stay noted, this is prevent the obsolete IPv4 feature in .NET IPAdress
            // However, we cut away the IPv6 section - This is because ENet does not support IPv6.
            // uint value = BitConverter.ToUInt32(ipEndPoint.Address.GetAddressBytes(), 0);         
            ENet.Address remoteAddr = new ENet.Address()
            {
                Host = (uint)ipEndPoint.Address.Address, 
                Port = remoteAddr.Port = (ushort)ipEndPoint.Port,
            };

            ENet.Connect(host, ref remoteAddr, (IntPtr)config.DefaultChannelAmount);
        }

        public void Disconnect(uint connectionId, uint flag)
        {
            if (connectionId > connections.Length)
            {
                Debug.Error(config.Name, ": Disconnect Id: ", connectionId.ToString(), " -- Out of connection array bounds");
                return;
            }

            ENet.DisconnectPeer(connections[connectionId].Peer, flag);
        }

        public void Disconnect(uint connectionId)
        {
            if (connectionId > connections.Length)
            {
                Debug.Error(config.Name, ": Disconnect Id: ", connectionId.ToString(), " -- Out of connection array bounds");
                return;
            }

            ENet.DisconnectPeer(connections[connectionId].Peer, 0);
        }
        #endregion
        #region Send Methods
        /// <summary>
        /// Queues a packet to be sent to all peers associated with the host.
        /// </summary>
        public void Broadcast(OutgoingMessage msg, byte channelId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroBroadcast(host, channelId, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Queues a packet to be sent to all peers associated with the host. Default channel = 0
        /// </summary>
        public void Broadcast(OutgoingMessage msg)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroBroadcast(host, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void Send(OutgoingMessage msg, uint connectionId, byte channelId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(connections[connectionId].Peer, channelId, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection. Default channel = 0
        /// </summary>
        public void Send(OutgoingMessage msg, uint connectionId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(connections[connectionId].Peer, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Send a message to a collection of remotes
        /// </summary>
        public void Send(OutgoingMessage msg, IList<RemoteConnection> remotes)
        {
            fixed (byte* bytes = msg.Data)
            {
                for(int i = remotes.Count; i == 0; i++)
                {
                    ENet.MicroSend(remotes[i].Peer, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
                }
            }
        }

        /// <summary>
        /// Send a message to a collection of remotes
        /// </summary>
        public void Send(OutgoingMessage msg, IList<int> connectionIds)
        {
            fixed (byte* bytes = msg.Data)
            {
                for (int i = connectionIds.Count; i == 0; i++)
                {
                    ENet.MicroSend(connections[i].Peer, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
                }
            }
        }
        #endregion


        internal void NATPunching(IPEndPoint addr)
        {
            ENet.Address address = new ENet.Address();
            address.Port = (ushort)config.Port;
            string strAddr = addr.Address.ToString();

            if (ENet.AddressSetHost(ref address, Encoding.ASCII.GetBytes(strAddr)) != 0)
            {
                Debug.Log(config.Name, " Failed to resolve host name");
            }

            Debug.Log(config.Name, " Punching: ", addr.Address.ToString(), " : ", address.Port.ToString());

            ENet.Connect(host, ref address, (IntPtr)config.DefaultChannelAmount);
            /*
            fixed (byte* bytes = msg.Data)
            {
                for (int i = 0; i < 8; i++)
                {
                    ENet.MicroSocketSend(host, ref address, bytes, (IntPtr)msg.ByteCount);
                    Thread.Sleep(5);
                }
            }

            if (!config.AllowConnectors)
            {
                ENet.Connect(host, ref address, (IntPtr)config.DefaultChannelAmount);
            }
            else
            {
                fixed (byte* bytes = msg.Data)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        ENet.MicroSocketSend(host, ref address, bytes, (IntPtr)msg.ByteCount);
                        Thread.Sleep(5);
                    }
                }
            }
            */

        }

        public abstract void OnConnect(RemoteConnection remote);
        public abstract void OnDisconnect(RemoteConnection remote);
        public abstract void OnReceived(IncomingMessage msg);
        public abstract void OnReady();
        public abstract void OnStop();
        public abstract void OnConnectionFailure();

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
                    if(msg.Remote == null)
                    {
                        OnConnectionFailure();
                        return;
                    }                       
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


            while (networkThread.ThreadState == System.Threading.ThreadState.Background)
            {
                while (isRunning)
                {
                    if (ENet.Service(host, out evt, serviceWait) > 0)
                    {
                        switch (evt.type)
                        {
                            case EventMessage.Connect:
                                {
                                    internalMsg = GetIncomingMessage();

                                    internalMsg.Remote = connections[evt.peer->incomingPeerID] = CreateRemoteConnection(evt.peer);
                                    internalMsg.Event = evt.type;

                                    IncomingEnqueue(internalMsg);

                                    connectionCount++;
                                    break;
                                }
                            case EventMessage.Disconnect:
                                {
                                    internalMsg = GetIncomingMessage();

                                    internalMsg.Event = evt.type;
                                    internalMsg.Remote = connections[evt.peer->incomingPeerID];

                                    IncomingEnqueue(internalMsg);

                                    connectionCount--;
                                    break;
                                }
                            case EventMessage.Receive:
                                {
                                    internalMsg = GetIncomingMessage();

                                    internalMsg.Type = evt.data;
                                    internalMsg.Event = evt.type;

                                    internalMsg.Remote = connections[evt.peer->incomingPeerID];

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

                if (host != null)
                {
                    ENet.DestroyHost(host);
                    connections = null;
                    host = null;

                    IncomingMessage stoppedEvent = GetIncomingMessage();
                    stoppedEvent.Event = EventMessage.NetworkStopped;
                    IncomingEnqueue(stoppedEvent);
                }
            }
        }

    }


}
