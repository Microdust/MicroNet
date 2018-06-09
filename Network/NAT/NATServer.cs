using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public class NATServer : NetworkManager
    {
        Dictionary<ulong, NatRemoteConnection> registeredHosts = new Dictionary<ulong, NatRemoteConnection>();


        public NATServer(NetConfiguration config, uint maxHostCount) : base(config)
        {
        }

        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Someone connected to the NAT server...");
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Someone disconnected from the NAT server...");
        }

        public override void OnReady()
        {
            Debug.Log(config.Name, ": NAT Server is ready...");
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch(msg.ReadUInt32())
            {
                case NATMessageType.INITIATE_HOST :
                Debug.Log(config.Name, ": Received host register request");
                NatRemoteConnection natHost = new NatRemoteConnection();
                natHost.Initialize(msg);

                registeredHosts[natHost.HostingId] = natHost;

                Debug.Log(config.Name, ": Host registered at: External IP: ", natHost.ExternalIp.ToString(), " and local IP: ", natHost.InternalIp.ToString(), " with following hosting ID: ", natHost.HostingId.ToString());

                break;

                case NATMessageType.GET_HOST_LIST:
                Debug.Log(config.Name, ": Request for a list of hosts was received");

                break;

                case NATMessageType.REQUEST_INTRODUCTION:
                NatRemoteConnection client = new NatRemoteConnection();
                client.Initialize(msg);

                Debug.Log(config.Name, ": Client requested introduction as: External IP: ", client.ExternalIp.ToString(), " and local IP: ", client.InternalIp.ToString(), " with following hosting ID: ", client.HostingId.ToString());

                Debug.Log(config.Name, ": Received introduction request to hostId: ", client.HostingId.ToString(), " and with the password: ", client.Password);

                NatRemoteConnection host;
                if (registeredHosts.TryGetValue(client.HostingId, out host))
                {
                    Debug.Log(config.Name, ": Host was found... Sending introduction");
                    byte[] addr = Encoding.ASCII.GetBytes("89.233.23.45");
                    
                    //Local test                   
                    client.ExternalIp.Address = new IPAddress(addr);

                    client.Introduce(host);
                    host.Introduce(client);
                }
           
                break;
            }
        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": Stopping...");
        }



        /// <summary>
        /// Called when host/client receives a NatIntroduction message from a master server
        /// </summary>
        internal void HandleNatIntroduction(int ptr)
        {



        }


    }
}
