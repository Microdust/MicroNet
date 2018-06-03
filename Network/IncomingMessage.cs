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
        public uint Type;
        public EventMessage Event;
        public RemoteConnection Remote = new RemoteConnection();

        public IncomingMessage(int byteSize) : base(byteSize) { }

        internal void Initialize(ENet.Event evt)
        {
            Type = evt.data;
            Event = evt.type;
            Remote.Peer = evt.peer;

            int length = (int)evt.packet->dataLength;

            if (length > Data.Length)
            {
                Debug.Log("Incoming Message array was too big, had to resize");
                Data = new byte[length];
            }

           
            Marshal.Copy(evt.packet->data, Data, 0, length);
        }

        internal void InitalizeEvent(ENet.Event evt)
        {
            Type = evt.data;
            Event = evt.type;

            Remote.Peer = evt.peer;
        }

        internal void Reset()
        {
            Type = 0;
            BitLocation = 0;
            BitLength = 0;
        }

        /*
        public byte[] GetBytes()
        {
            int length = (int)Packet->dataLength;
            byte[] bytes = new byte[length];

            Marshal.Copy(Packet->data, bytes, 0, length);

            return bytes;
        }
        */
        
    }
}
