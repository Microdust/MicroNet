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

        public EventMessage Event;
        public RemoteConnection Remote = new RemoteConnection();
        internal int BitLocation = 0;

        public IncomingMessage(int byteSize) : base(byteSize) { }

        internal void Reset()
        {
            Type = 0;
            BitLocation = 0;
            BitLength = 0;

        }
        
    }
}
