using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public abstract unsafe class Message
    {       
        public DeliveryMethod DeliveryMethod;
        internal byte[] Data;

        internal int BitLength;
        internal int ByteCount;

        public uint Type;

        public Message(int byteSize)
        {
            Data = new byte[byteSize];
            ByteCount = byteSize;
        }

        public void Recycle()
        {
            BitLength = 0;
            DeliveryMethod = 0;

            if (ByteCount > 56 )
            {
                Array.Copy(NetUtilities.EmptyByteArray, Data, ByteCount);
                ByteCount = 0;
                return;
            }

            for (int i = 0; i < ByteCount; i++)
            {
                Data[i] = 0;
            }
            ByteCount = 0;
        }

    }
}
