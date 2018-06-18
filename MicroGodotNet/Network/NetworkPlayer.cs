using MicroGodotNet.Messages;
using MicroGodotNet.Signal;
using MicroNet.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet
{
    public abstract class NetworkPlayer : NetworkManager
    {

        protected NetworkPlayer(NetConfiguration config) : base (config)
        {          
        }



        public abstract void Send(Movement movement);

    }
}
