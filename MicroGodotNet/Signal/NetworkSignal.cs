using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Signal
{
    public enum NetworkStatus
    {
        ConnectionSuccess,
        ConnectionFailure,
        Disconnection,
        Hosting,
        Ready,
        Stopped
    }

    public class NetworkInformation
    {
        public NetworkStatus Status;
        public IPEndPoint EndPoint;

    }

}
