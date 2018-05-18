using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public abstract unsafe class Message : IDisposable
    {
        public ENet.ENetPacket* Packet;
        public DeliveryMethod DeliveryMethod;
        internal byte[] Data;


        internal int BitLocation = 0;
        internal int ByteLength = 0;

        public void Dispose()
        {
            Packet = null;
            Data = null;
            ENet.DestroyPacket(Packet);
        }

        public void EnsureBufferSize(int bitCount)
        {
            
        }

    }
}
