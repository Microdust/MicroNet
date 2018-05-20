using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe partial class IncomingMessage : Message
    {

        public void Initialize(ENet.ENetEvent evt)
        {
            this.Packet = evt.packet;

            IntPtr srcPtr = (IntPtr)((byte*)Packet->data + 0);
            Marshal.Copy(srcPtr, Data, 0, (int)Packet->dataLength);
        }

        public byte[] GetBytes()
        {
            int length = (int)Packet->dataLength;
            byte[] bytes = new byte[length];
            IntPtr srcPtr = (IntPtr)((byte*)Packet->data + 0);
            Marshal.Copy(srcPtr, bytes, 0, length);

            return bytes;
        }

    }
}
