using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{
    public unsafe struct NATServerConnection
    {
        internal ENet.Peer* Peer;
        private ENet.Host* Host;
        private IPEndPoint endPoint;

        internal NATServerConnection(ENet.Host* host, IPEndPoint target)
        {
            Host = host;
            endPoint = target;
            Peer = null;
        }

        public bool IsConnected
        {
            get
            {
                return Peer->state == Network.ConnectionState.Connected;
            }
        }

   
        public uint ConnectionId
        {
            get { return Peer->incomingPeerID; }
        }

        /// <summary>
        /// Returns the IPEndPoint of the remote NAT Server
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                endPoint.Address.Address = Peer->address.Host;
                endPoint.Port = Peer->address.Port;
                return endPoint;
            }
        }

        /// <summary>
        /// Returns the round trip time for the remote connection
        /// </summary>
        public uint RTT
        {
            get { return Peer->roundTripTime; }
        }

        /// <summary>
        /// Returns the time since last message was sent from the remote host (check if true)
        /// </summary>
        public uint TimeSinceLastMessage
        {
            get { return Peer->lastSendTime; }
        }


        /// <summary>
        /// Force an additional ping to a remote connection 
        /// </summary>
        public void Ping()
        {
            ENet.PingPeer(Peer);
        }


        /// <summary>
        /// Sends a message to a remote connection. Default channel = 0
        /// </summary>
        public void RequestHostList()
        {
            OutgoingMessage regMessage = MessagePool.CreateMessage();
            regMessage.Write(NATMessageType.REQUEST_HOST_LIST);
            

            fixed (byte* bytes = regMessage.Data)
            {
                ENet.MicroSend(Peer, 0, bytes, (IntPtr)regMessage.ByteCount, DeliveryMethod.Reliable);
            }
        }


        /// <summary>
        /// Sends a message to a remote connection. Default channel = 0
        /// </summary>
        public void RegisterHosting()
        {
            OutgoingMessage regMessage = MessagePool.CreateMessage();
            IPAddress local = NetUtilities.GetLocalAddress();

            regMessage.Write(NATMessageType.INITIATE_HOST);                
            regMessage.WriteString("hello");

            fixed (byte* bytes = regMessage.Data)
            {
                ENet.MicroSend(Peer, 0, bytes, (IntPtr)regMessage.ByteCount, DeliveryMethod.Reliable);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void RequestIntroduction(NATHostInfo info)
        {
            OutgoingMessage request = MessagePool.CreateMessage();
          //  IPAddress local = NetUtilities.GetLocalAddress();
            request.Write(NATMessageType.REQUEST_INTRODUCTION);
            request.Write(info.HostId);


            fixed (byte* bytes = request.Data)
            {
                ENet.MicroSend(Peer, 0, bytes, (IntPtr)request.ByteCount, DeliveryMethod.Reliable);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void InformPunchthroughSuccess()
        {
            OutgoingMessage request = MessagePool.CreateMessage();
            IPAddress local = NetUtilities.GetLocalAddress();
            request.Write(NATMessageType.NAT_PUNCH_SUCCESS);

            fixed (byte* bytes = request.Data)
            {
                ENet.MicroSend(Peer, 0, bytes, (IntPtr)request.ByteCount, DeliveryMethod.Reliable);
            }
        }

        public void Connect()
        {

            ENet.Address remoteAddr = new ENet.Address()
            {
                Host = (uint)endPoint.Address.Address,
                Port = remoteAddr.Port = (ushort)endPoint.Port
            };

            Peer = ENet.Connect(Host, ref remoteAddr, (IntPtr)1);
        }

        ///<summary>
        /// Disconnects from a remote connection
        /// </summary>
        public void Disconnect()
        {
            ENet.DisconnectPeer(Peer, 1);
        }

        ///<summary>
        /// Resets the connection without sending any disconnect events
        /// </summary>
        public void Reset()
        {
            ENet.ResetPeer(Peer);
        }

        ///<summary>
        /// Adjust the ping interval
        /// </summary>
        public void SetPingInterval(uint interval)
        {
            Peer->pingInterval = interval;
        }

        ///<summary>
        /// Current state of the connection
        /// </summary>
        public ConnectionState ConnectionState()
        {
            return Peer->state;
        }
    }
}
