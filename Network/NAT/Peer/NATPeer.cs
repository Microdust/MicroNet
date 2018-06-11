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


        public NATPeer(NetConfiguration config) : base(config)
        {
        }

        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, " Connected to: ", remote.IPAddress.ToString());
            
            if(config.Name == "NATHost")
            {
                relayServer.RegisterHosting(config.Port);
            }
            else
            {
                relayServer.RequestIntroduction(0, 0);
            }
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log("Disconnected from: ", remote.IPAddress.ToString());
        }

        public override void OnReady()
        {
            Debug.Log(config.Name, ": Connecting to NAT relay server...");


            relayServer = ConnectToNATServer();

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

                case NATMessageType.NAT_PUNCHTHROUGH:
                Debug.Log(config.Name, ": NAT PUNCH THROUGH...");
                break;

            }
        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": Stopped...");
        }

        public void HandleAck(IncomingMessage msg)
        {
            Debug.Log(config.Name, ": Received acknowledgement from master server. Listed for NAT punchthrough...");
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

            NATPunching(remoteExternal);

        }

    }
}
