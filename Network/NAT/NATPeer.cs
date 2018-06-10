using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe class NATPeer : NetworkManager
    {
        private uint hostId = 5000;


        public NATPeer(NetConfiguration config) : base(config)
        {


        }

        public override void OnConnect(RemoteConnection remote)
        {
            Debug.Log(config.Name, " Connected to: ", remote.IPAddress.ToString());
            
            if(config.Name == "NATHost")
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
            Debug.Log(config.Name, ": Connecting to NAT relay server...");

            byte[] IP = { 127, 0, 0, 1 };
            IPEndPoint point = new IPEndPoint(new IPAddress(IP), 5000);

            Connect(point);

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

            IPEndPoint remoteInternal = msg.ReadIPEndPoint();
            IPEndPoint remoteExternal = msg.ReadIPEndPoint();
            string Password = msg.ReadString();

            Debug.Log(config.Name, ": was introduced to external IP: ", remoteExternal.ToString(), ", internal IP: ", remoteInternal.ToString(), " and the password: ", Password);

            NATPunching(remoteExternal);

            //Connect(remoteExternal.Address, (ushort)remoteExternal.Port);
    

            /* OutgoingMessage outgoing = MessagePool.CreateMessage();
             outgoing.Write(NATMessageType.NAT_PUNCHTHROUGH);
             outgoing.WriteString(Password);

             msg.Remote.ChangeAddress(remoteInternal.Address, 8080);
             msg.Remote.Send(outgoing);
             msg.Remote.ChangeAddress(remoteExternal.Address, 8080);
             msg.Remote.Send(outgoing);
             */

            //   if (!isHost && m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess) == false)
            //       return; // no need to punch - we're not listening for nat intros!

            // send internal punch

        }


        public void RegisterHosting(RemoteConnection remote)
        {
            OutgoingMessage regMessage = MessagePool.CreateMessage();
            regMessage.DeliveryMethod = DeliveryMethod.Reliable;

            regMessage.Write(NATMessageType.INITIATE_HOST);

            IPAddress local = NetUtilities.GetLocalAddress();

            Debug.Log(config.Name, ": using local IP: ", local.ToString());
            regMessage.Write(new IPEndPoint(local, config.Port));

            regMessage.Write(hostId); // HostID slash identifier

            regMessage.WriteString("hello");

            remote.Send(regMessage);

        }

        public void RequestNATIntroduction(RemoteConnection remote)
        {
            OutgoingMessage request = MessagePool.CreateMessage();
            request.DeliveryMethod = DeliveryMethod.Reliable;

            request.Write(NATMessageType.REQUEST_INTRODUCTION);

            // write my internal ipendpoint
            IPAddress local = NetUtilities.GetLocalAddress();

            Debug.Log(config.Name, ": using local IP: ", local.ToString());
            request.Write(new IPEndPoint(local, config.Port));

            // write requested host id
            request.Write(hostId);

            // write token
            request.WriteString("hello");

            remote.Send(request);
        }

    }
}
