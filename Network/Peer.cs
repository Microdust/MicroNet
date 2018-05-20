using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe struct Peer
    {
        public ENet.ENetPeer* ENetPeer;
    
        public Peer(ENet.ENetPeer* peer)
        {
            this.ENetPeer = peer;
        }
        
        public void Disconnect(uint data)
        {
            ENet.DisconnectPeer(ENetPeer, data);
        }

        public uint RoundTripTime
        {
            get { return ENetPeer->lastRoundTripTime; }
        }
    }
}
