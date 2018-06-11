﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe struct RemoteConnection
    {
        internal ENet.Peer* Peer;
        internal IPEndPoint EndPoint;

        public uint ConnectionId
        {
            get { return Peer->incomingPeerID; }
        }

        public void SetRemote(RemoteConnection conn)
        {
            Peer = conn.Peer;

        }



        internal RemoteConnection(ENet.Peer* peer)
        {
            Peer = peer;       
            EndPoint = new IPEndPoint(new IPAddress(peer->address.Host), peer->address.Port);
        }

        /// <summary>
        /// Returns the IP address of the remote connection
        /// </summary>
        public IPAddress IPAddress
        {
            get { return new IPAddress(Peer->address.Host); }
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
