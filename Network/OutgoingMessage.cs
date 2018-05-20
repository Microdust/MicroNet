using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe partial class OutgoingMessage : Message
    {

        public OutgoingMessage(DeliveryMethod deliveryMethod)
        {
            DeliveryMethod = deliveryMethod;
        }

        public OutgoingMessage()
        {
        }

        public void Write(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            BitLength =+ bytes.Length;

            Data = bytes;
        }


        public ENet.ENetPacket* GetPacket()
        {

            fixed (byte* bytes = Data)
            {
                return Packet = ENet.CreatePacket(bytes, (IntPtr)Data.Length, DeliveryMethod);
            }               
        }




    }
}
