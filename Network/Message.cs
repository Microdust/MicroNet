using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public abstract unsafe class Message
    {
        internal NetSingle Single = new NetSingle();

        internal ENet.Packet* Packet;
        public DeliveryMethod DeliveryMethod;
        internal byte[] Data;

        internal int BitLocation = 0;
        internal int BitLength = 0;

        public Message(int byteSize)
        {
            Data = new byte[byteSize];
        }


    }
}
