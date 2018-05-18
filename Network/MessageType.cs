using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public enum MessageType
    {
        None = 0,
        Connect = 1,
        Disconnect = 2,
        Receive = 3
    }
}
