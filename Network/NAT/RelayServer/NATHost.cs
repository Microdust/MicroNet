using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network.NAT
{ 
    public class NATHost : NATRemote
    {
        public NATHostInfo Info = new NATHostInfo();

        public NATHost(IncomingMessage msg) : base(msg.Remote)
        {


        }
    }
}
