using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{

    public unsafe class Host
    {
        private ENet.ENetHost* ENetHost;
        private NetConfiguration config;

        public ENet.ENetPeer*[] Connections;

        private IncomingMessage msg = new IncomingMessage();
        private IncomingMessage nullMessage;

        private ENet.ENetEvent evt;

        HashSet<Peer> hashConnections = new HashSet<Peer>();

        public Host(NetConfiguration configuration)
        {
            this.config = configuration;
            this.Connections = new ENet.ENetPeer*[config.MaxConnections];
        }

        public void Initialize()
        {
            if (ENetHost != null)
            {
                Debug.Error(this.ToString(), " Attempted to create a host but one was already established.");
            }

            //   CheckChannelLimit(channelLimit);

            if (config.AllowConnectors)
            {
                ENet.ENetAddress address = new ENet.ENetAddress();
                address.Port = config.Port;
                

                ENet.AddressSetHost(ref address, config.LocalAddress);
                ENetHost = ENet.CreateHost(ref address, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth);  
            }
            else
            {
                ENetHost = ENet.CreateHost(null, (IntPtr)config.MaxConnections, (IntPtr)config.MaxConnections, config.IncomingBandwidth, config.OutgoingBandwidth);
            }

            if (ENetHost == null)
            {
                Debug.Error(this.ToString(), " Failed to create host");
            }
        }

        public void Connect(string address, ushort port)
        {
            ENet.ENetAddress ENetAddress = new ENet.ENetAddress();
            ENetAddress.Port = port;

            Debug.Log("Attempting to connect to: ", address, ":", port.ToString());
            ENet.AddressSetHost(ref ENetAddress, Encoding.ASCII.GetBytes(address));

            Peer connection = new Peer(ENet.ConnectHost(ENetHost, ref ENetAddress, (IntPtr)config.MaxConnections, config.AppIdentification));
     

            if (connection.ENetPeer != null)
            {
                hashConnections.Add(connection);
            }
            else
            {
                Debug.Error(this.ToString(), " Failed to connect");
            }
         
        }

        /// <summary>Sends any queued packets on the host specified to its designated peers. </summary>
        /// <remarks>This function need only be used in circumstances where one wishes to send queued packets earlier than in a call to enet_host_service()</remarks>
        public void Flush()
        {
            ENet.FlushHost(ENetHost);
        }

        public void Send(OutgoingMessage msg, ENet.ENetPeer* receiver, byte channelId)
        {
            ENet.SendPeer(receiver, channelId, msg.GetPacket());
        }

        /// <summary>
        /// Queues a packet to be sent to all peers associated with the host.
        /// </summary>
        public void Broadcast(OutgoingMessage msg, byte channelId)
        {
            ENet.BroadcastHost(ENetHost, channelId, msg.GetPacket());
        }

        public IncomingMessage Service()
        {
            if (config.Timeout < 0)
            {
                Debug.Error(this.ToString(), " Timeout service");
            }

            if (ENet.ServiceHost(ENetHost, out evt, config.Timeout) > 0)
            {

                switch (evt.type)
                {
                    case MessageType.Connect:
                    {
                        Debug.Log(config.Name, " Connected");
                    //    hashConnections.Add(new Peer(evt.peer));

                        OutgoingMessage outgoing = new OutgoingMessage(DeliveryMethod.None);
                            outgoing.WriteByte(200);
                            outgoing.WriteBool(false);

                            outgoing.WriteBool(true);
                            outgoing.WriteByte(123);
                            outgoing.WriteByte(55);

                            ENet.SendPeer(evt.peer, 0, outgoing.GetPacket());

                        break;
                    }
                    case MessageType.Disconnect:
                    {
                    
                        Debug.Log(config.Name, " Disconnected");
                        break;
                    }
                    case MessageType.Receive:
                    {
                        Debug.Log(config.Name, " Received data");
                        msg.Initialize(evt);

                        return msg;
                    }

                }
           
            }

            return nullMessage;
        }


        public void Shutdown()
        {
            ENet.DestroyHost(ENetHost);
        }

    }


}
