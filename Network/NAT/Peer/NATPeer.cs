using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public partial class NATPeer : NetworkManager
    {
        private NATServerConnection relayServer;
        private List<NATHostInfo> hosts = new List<NATHostInfo>();


        public NATPeer(NetConfiguration config) : base(config)
        {
        }

        public override void OnConnect(RemoteConnection remote)
        {
            Console.WriteLine("A connection was established");
            
            if (config.AllowConnectors)
            {
                relayServer.RegisterHosting();
            }
            else
            {
                relayServer.RequestHostList();
            }

        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log("Disconnected from: ", remote.EndPoint.ToString());
        }

        public override void OnReady()
        {
            Debug.Log(config.Name, ": Connecting to NAT relay server...");


            relayServer = CreateNATServerConnection();
            relayServer.Connect();

        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch (msg.ReadUInt32())
            {
                case NATMessageType.ACK:
                HandleAck(msg);
                break;

                case NATMessageType.INTRODUCTION:
                HandleNatIntroduction(msg);
                break;

                case NATMessageType.REQUEST_HOST_LIST:
                HandleGetHostList(msg);
                break;

            }
        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": Stopped...");
        }

        public void HandleAck(IncomingMessage msg)
        {
             byte[] localHost = { 127, 0, 0, 1 };

        Debug.Log(config.Name, ": Received acknowledgement from master server. Listed for NAT punchthrough...");

            StopNetwork();
            /*

            ModifyNetwork(new NetConfiguration(5001)
            {
                Port = 5555,
                AllowConnectors = true,
                MaxConnections = 5,
                Name = "NATHost",
                Timeout = 1,
                NetworkRate = 50,
                ServerEndPoint = new IPEndPoint(new IPAddress(localHost), 5000)
            });
            */
        }

        /// <summary>
        /// Called when host/client receives a NatIntroduction message from a master server
        /// </summary>
        internal void HandleNatIntroduction(IncomingMessage msg)
        {
//            IPEndPoint remoteInternal = msg.ReadIPEndPoint();
            IPEndPoint remoteExternal = msg.ReadIPEndPoint();
            string Password = msg.ReadString();

            Debug.Log(config.Name, ": was introduced to external IP: ", remoteExternal.ToString(), " and the password: ", Password);

            if(!config.AllowConnectors)
            {
                NATPunching(remoteExternal);
            }
        }

        internal void HandleGetHostList(IncomingMessage msg)
        {
            int count = msg.ReadInt32();

            Debug.Log(config.Name, " Received host list. The count was: ", count.ToString());

            for (int i = 0; i < count; i++)
            {
                NATHostInfo hostInfo = new NATHostInfo();

                hostInfo.ReadMessage(msg);

                hosts.Add(hostInfo);
                Debug.Log(hostInfo.HostId.ToString(), " , ", hostInfo.Title);
            }
            // Test
            relayServer.RequestIntroduction(hosts[0].HostId);

        }

        public override void OnConnectionFailure()
        {
            Debug.Log(config.Name, ": OnConnectionFailure");
        }
    }
}
