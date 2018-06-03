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


        public ENet.Packet* GetPacket()
        {

            fixed (byte* bytes = Data)
            {
                Packet = ENet.CreatePacket(bytes, (IntPtr)Data.Length, DeliveryMethod);
            }

            this.BitLength = 0;
            this.BitLocation = 0;
        //    this.Data = emptyData;

            return Packet;
        }
        

    }
}
