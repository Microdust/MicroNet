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

        public OutgoingMessage(DeliveryMethod deliveryMethod, int byteSize) : base(byteSize)
        {
            DeliveryMethod = deliveryMethod;
        }

        public OutgoingMessage(int byteSize) : base(byteSize)
        {
        }       

    }
}
