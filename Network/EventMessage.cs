using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public enum EventMessage
    {
        None = 0,
        Connect = 1,
        Disconnect = 2,
        Receive = 3,

        NetworkReady = 127,
        NetworkStopped = 128
            

        
    }
}
