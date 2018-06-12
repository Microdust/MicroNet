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
            Debug.Log(config.Name, ": Established connection to: ", remote.EndPoint.ToString());         
        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Disconnected from: ", remote.EndPoint.ToString());
        }

        public override void OnReady()
        {
            Debug.Log(config.Name, ": NAT Server is ready");
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch(msg.ReadUInt32())
            {
                case NATMessageType.INITIATE_HOST:
                HandleHostSetup(msg);
                break;

                case NATMessageType.REQUEST_HOST_LIST:
                HandleHostListRequest(msg.Remote);               
                break;

                case NATMessageType.REQUEST_INTRODUCTION:
                HandleIntroductionRequest(msg);
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


        public void HandleHostSetup(IncomingMessage msg)
        {
            Debug.Log(config.Name, ": Received host register request");
            NATHost natHost = new NATHost(msg);
            Debug.Log(config.Name, ": Adding host as id: ", natHost.Info.HostId.ToString());
            registeredHosts.Add(natHost.Info.HostId, natHost);

            Debug.Log(config.Name, ": Host registered at: External IP: ", natHost.Remote.EndPoint.ToString());

            OutgoingMessage ackMsg = MessagePool.CreateMessage();
            ackMsg.Write(NATMessageType.ACK);
            msg.Remote.Send(ackMsg);
        }

        public void HandleIntroductionRequest(IncomingMessage msg)
        {
            NATRemote client = new NATRemote(msg.Remote);

            //Local test           
            byte[] addr = { 89, 233, 23, 45 };
            client.Remote.EndPoint.Address = new IPAddress(addr); // Temp

            ulong hostId = msg.ReadUInt64();

            Debug.Log(config.Name, ": Client requested introduction as: External IP: ", client.Remote.EndPoint.ToString(), " and local IP: ");
            Debug.Log(config.Name, ": Received introduction request to hostId: ", hostId.ToString());

            if (registeredHosts.TryGetValue(hostId, out NATHost host))
            {
                Debug.Log(config.Name, ": Host was found... Sending introduction");

                host.Introduce(client);
                client.Introduce(host);
            }
        }


        public void HandleNatPunchSuccess(RemoteConnection remote)
        {
                Debug.Log(config.Name, ": Received Punch Success");

            remote.Disconnect();                         
        }

        public void HandleHostListRequest(RemoteConnection remote)
        {
            Debug.Log(config.Name, ": Request for a list of hosts was received");
            OutgoingMessage listMsg = MessagePool.CreateMessage();

            listMsg.Write(NATMessageType.REQUEST_HOST_LIST);
            listMsg.Write(registeredHosts.Count);

            foreach(NATHost item in registeredHosts.Values)
            {
                item.Info.WriteMessage(listMsg);
            }
       

            remote.Send(listMsg);

        }

        private void Introduce(RemoteConnection remote , IPEndPoint endPoint)
        {
            Debug.Log("NATServer: Introducing: ", endPoint.ToString(), " to: ", remote.EndPoint.ToString());
            OutgoingMessage message = MessagePool.CreateMessage();
            message.Write(NATMessageType.INTRODUCTION);
            message.Write(endPoint);
            remote.Send(message);

            MessagePool.Recycle(message);
        }


    }
}
