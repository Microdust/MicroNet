using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe class RemoteConnection
    {
        internal ENet.Peer* Peer;
        internal IPEndPoint internalEndPoint = new IPEndPoint(0, 0);

        internal void Initialize(ENet.Peer* peer)
        {
            Peer = peer;
            internalEndPoint.Address.Address = Peer->address.Host;
            internalEndPoint.Port = Peer->address.Port;
        }

        internal RemoteConnection(ENet.Peer* peer)
        {
            Peer = peer;
            internalEndPoint = new IPEndPoint(Peer->address.Host, Peer->address.Port);
        }

        internal RemoteConnection() { }


        /// <summary>
        /// Returns the id of the connection
        /// </summary>
        public uint ConnectionId
        {
            get { return Peer->incomingPeerID; }
        }

        /// <summary>
        /// Returns the ping interval (one-way) in milliseconds
        /// </summary>
        public uint PingInterval
        {
            get { return Peer->pingInterval; }
        }

        /// <summary>
        /// Returns the IP address of the remote connection
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return internalEndPoint; }
        }

        /// <summary>
        /// Returns the downstream bandwidth of the remote connection in bytes per second
        /// </summary>
        public uint BandwidthDownstream
        {
            get { return Peer->incomingBandwidth; }
        }

        /// <summary>
        /// Returns the upstream bandwidth of the remote connection in bytes per second
        /// </summary>
        public uint BandwidthUpstream
        {
            get { return Peer->outgoingBandwidth; }
        }

        /// <summary>
        /// Returns the total downstream bandwidth in bytes of the remote connection 
        /// </summary>
        public uint BandwidthTotalDownstream
        {
            get { return Peer->incomingDataTotal; }
        }

        /// <summary>
        /// Returns the total upstream bandwidth in bytes of this remote connection 
        /// </summary>
        public uint BandwidthTotalUpstream
        {
            get { return Peer->outgoingDataTotal; }
        }

        /// <summary>
        /// Returns the round trip time (RTT), measured in milliseconds
        /// </summary>
        public uint RTT
        {
            get { return Peer->roundTripTime; }
        }

        /// <summary>
        /// Returns the lowest round trip time (RTT), measured in milliseconds
        /// </summary>
        public uint RTTLowest
        {
            get { return Peer->lowestRoundTripTime; }
        }

        /// <summary>
        /// Returns the total amount of packets lost
        /// </summary>
        public uint TotalPacketsLost
        {
            get { return Peer->packetsLost; }
        }

        /// <summary>
        /// Returns the time since last message was sent from the remote host (check if true)
        /// </summary>
        public uint TimeSinceLastMessage
        {
            get { return Peer->lastSendTime; }
        }

        /// <summary>
        /// Returns the maximum transmission unit (MTU)
        /// </summary>
        public uint MTU
        {
            get { return Peer->mtu; }
        }

        /// <summary>
        /// Returns the amount of available channels
        /// </summary>
        public uint ChannelCount
        {
            get { return (uint)Peer->channelcount; }
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
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(Peer, 0, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        /// <summary>
        /// Sends a message to a remote connection
        /// </summary>
        public void Send(OutgoingMessage msg, byte channelId)
        {
            fixed (byte* bytes = msg.Data)
            {
                ENet.MicroSend(Peer, channelId, bytes, (IntPtr)msg.ByteCount, msg.DeliveryMethod);
            }
        }

        ///<summary>
        /// Disconnects from a remote connection with an id message
        /// </summary>
        public void Disconnect(uint id)
        {
            ENet.DisconnectPeer(Peer, id);
        }

        ///<summary>
        /// Disconnects from a remote connection
        /// </summary>
        public void Disconnect()
        {
            ENet.DisconnectPeer(Peer, 0);
        }

        ///<summary>
        /// Disconnects from a remote connection unreliably
        /// </summary>
        public void DisconnectForcefully()
        {
            ENet.DisconnectPeerNow(Peer, 0);
        }

        ///<summary>
        /// Disconnects from a remote connection unreliably with an id message
        /// </summary>
        public void DisconnectForcefully(uint id)
        {
            ENet.DisconnectPeerNow(Peer, id);
        }

        ///<summary>
        /// Resets the connection without sending any disconnect events. This will cause a timeout event
        /// </summary>
        internal void Reset()
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
