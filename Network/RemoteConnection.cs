using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe sealed class RemoteConnection
    {
        internal ENet.Peer* Peer;
        public bool IsConnected = true;

        public uint ConnectionId
        {
            get { return Peer->incomingPeerID; }
        }


        /// <summary>
        /// Returns the IP address of the remote peer
        /// </summary>
        public IPAddress IPAddress()
        {
            return new IPAddress(Peer->address.Host);   
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
        public void Send(OutgoingMessage msg)
        {           
            ENet.SendPeer(Peer, 0, msg.GetPacket());
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void Send(OutgoingMessage msg, byte channelId)
        {
            ENet.SendPeer(Peer, channelId, msg.GetPacket());
        }

        ///<summary>
        /// Disconnects from a remote connection
        /// </summary>
        public void Disconnect()
        {
            ENet.DisconnectPeer(Peer, 1337);
        }

        ///<summary>
        /// Resets the connection without sending any disconnect events
        /// </summary>
        public void Reset()
        {
            ENet.ResetPeer(Peer);
        }

        ///<summary>
        /// To be implemented
        /// </summary>
        public void SetPingInterval(int interval)
        {       
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
