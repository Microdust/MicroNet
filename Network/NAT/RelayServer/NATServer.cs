using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public class NATServer : NetworkManager
    {
        private Dictionary<ulong, NATHost> registeredHosts = new Dictionary<ulong, NATHost>();
        

        public NATServer(NetConfiguration config, uint maxHostCount) : base(config)
        {
            
        }


        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Established connection to: ", remote.IPAddress.ToString());         
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Disconnected from: ", remote.IPAddress.ToString());
        }

        public override void OnReady()
        {
            Debug.Log(config.Name, ": NAT Server is ready");
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch(msg.ReadUInt32())
            {
                case NATMessageType.INITIATE_HOST :
                Debug.Log(config.Name, ": Received host register request");
                NATHost natHost = new NATHost();
                natHost.Initialize(msg);
                registeredHosts.Add(natHost.HostId, natHost);

                Debug.Log(config.Name, ": Host registered at: External IP: ", natHost.External.ToString(), " and local IP: ");

                OutgoingMessage ackMsg = MessagePool.CreateMessage();
                ackMsg.Write(NATMessageType.ACK);
                msg.Remote.Send(ackMsg);

                break;

                case NATMessageType.GET_HOST_LIST:
                Debug.Log(config.Name, ": Request for a list of hosts was received");

                break;

                case NATMessageType.REQUEST_INTRODUCTION:
                NATClient client = new NATClient();
                client.Initialize(msg);

                //Local test           
                byte[] addr = { 89, 233, 23, 45 };
                client.External.Address = new IPAddress(addr);

                Debug.Log(config.Name, ": Client requested introduction as: External IP: ", client.External.ToString(), " and local IP: ");

                Debug.Log(config.Name, ": Received introduction request to hostId: ", client.HostId.ToString());

                NATHost host;    
                if (registeredHosts.TryGetValue(client.HostId, out host))
                {
                    Debug.Log(config.Name, ": Host was found... Sending introduction");
                    

                    host.Introduce(client);
                    client.Introduce(host);

                }

                break;

                case NATMessageType.NAT_PUNCH_SUCCESS:
                    HandleNatPunchSuccess(msg.Remote);
                break;
            }
        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": Stopping...");
        }

        public void HandleNatPunchSuccess(RemoteConnection remote)
        {
            if (registeredHosts.Remove(NetUtilities.CreateUniqueId(remote.EndPoint)))
            {
                Debug.Log("Removed Host from list");
            }

            remote.Disconnect();                         
        }



        /// <summary>
        /// Called when host/client receives a NatIntroduction message from a master server
        /// </summary>
        internal void HandleNatIntroduction(int ptr)
        {



        }

    }
}
