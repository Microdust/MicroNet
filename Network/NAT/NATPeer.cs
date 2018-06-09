using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public class NATPeer : NetworkManager
    {
        private uint hostId = 5000;


        public NATPeer(NetConfiguration config) : base(config)
        {


        }

        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log("Connected to: ", remote.IPAddress.ToString());

            if(config.AllowConnectors)
            {
                RegisterHosting(remote);
            }
            else
            {
                RequestNATIntroduction(remote);
            }

        }

        public override void OnDisconnect(RemoteConnection remote)
        {
            Debug.Log("Disconnected from: ", remote.IPAddress.ToString());
        }

        public override void OnReady()
        {
            Connect("127.0.0.1", 8080);
        }

        public override void OnReceived(IncomingMessage msg)
        {
            switch (msg.ReadUInt32())
            {
                case NATMessageType.INTRODUCTION:
                HandleNatIntroduction(msg);
                break;


            }
        }

        public override void OnStop()
        {
            Debug.Log(config.Name, ": Stopped...");
        }

        /// <summary>
        /// Called when host/client receives a NatIntroduction message from a master server
        /// </summary>
        internal void HandleNatIntroduction(IncomingMessage msg)
        {
            
            bool isHost = msg.ReadBool();
            IPEndPoint remoteInternal = msg.ReadIPEndPoint();
            IPEndPoint remoteExternal = msg.ReadIPEndPoint();
            string Password = msg.ReadString();

            OutgoingMessage outgoing = MessagePool.CreateMessage();

            //   if (!isHost && m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess) == false)
            //       return; // no need to punch - we're not listening for nat intros!

            // send internal punch

            Connect(remoteInternal.Address, 8080);
            Connect(remoteExternal.Address, 8080);

        }

        public void RegisterHosting(RemoteConnection remote)
        {
            OutgoingMessage regMessage = MessagePool.CreateMessage();
            regMessage.DeliveryMethod = DeliveryMethod.Reliable;

            regMessage.Write(NATMessageType.INITIATE_HOST);

            IPAddress local = NetUtilities.GetLocalAddress();
            Debug.Log("local: ", local.ToString());
            regMessage.Write(new IPEndPoint(local, config.Port));

            regMessage.Write(hostId); // HostID slash identifier


            remote.Send(regMessage);

        }

        public void RequestNATIntroduction(RemoteConnection remote)
        {
            OutgoingMessage request = MessagePool.CreateMessage();
            request.DeliveryMethod = DeliveryMethod.Reliable;
            request.Write(NATMessageType.REQUEST_INTRODUCTION);

            // write my internal ipendpoint
            IPAddress local = NetUtilities.GetLocalAddress();

            Debug.Log("local: ", local.ToString());
            request.Write(new IPEndPoint(local, 8080));

            // write requested host id
            request.Write(hostId);

            // write token
            request.WriteString("mytoken");

            remote.Send(request);
        }

    }
}
