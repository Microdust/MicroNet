using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe partial class OutgoingMessage : Message
    {
        private byte[] emptyData = new byte[16];

        public OutgoingMessage(DeliveryMethod deliveryMethod, int byteSize) : base(byteSize)
        {
            DeliveryMethod = deliveryMethod;

        }

        public OutgoingMessage(int byteSize) : base(byteSize)
        {
        }
        /*
        public ENet.Packet* Packet
        {
            get
            {
                BitLength = 0;
                BitLocation = 0;

                fixed (byte* bytes = Data)
                {
                    return ENet.CreatePacket(bytes, (IntPtr)Data.Length, DeliveryMethod);
                }
            }
        }
        */
    }
}
