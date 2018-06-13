using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public sealed class NetConfiguration
    {

        /// <summary>
        /// Max connections allowed
        /// </summary>
        public int MaxConnections = 12;

        /// <summary>
        /// Name of the connection owner
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Port
        /// </summary>
        public ushort Port = 9999;

        /// <summary>
        /// The name of the application which should connect
        /// </summary>
        public uint AppIdentification;

        /// <summary>
        /// Number of milliseconds that ENet waits for events. Defaults to 1.
        /// </summary>
        public uint Timeout = 1;

        /// <summary>
        /// Downstream bandwidth of the host in bytes/second; if 0 = unlimited bandwidth.
        /// </summary>
        public uint IncomingBandwidth = 0;

        /// <summary>
        /// Upstream bandwidth of the host in bytes/second; if 0 = unlimited bandwidth.
        /// </summary>
        public uint OutgoingBandwidth = 0;

        /// <summary>
        /// Allow others to connect. For example, set true if hosting.
        /// </summary>
        public bool AllowConnectors = false;

        /// <summary>
        /// The local address of the network device in bytes.
        /// </summary>
        public byte[] LocalAddress;

        /// <summary>
        /// How often the network thread should attempt to update per second. 
        /// </summary>
        public int NetworkRate = 50;

        /// <summary>
        /// The allocated size of the incoming message buffer. If the buffer size exceeds it will resize the array which is performance-heavy.
        /// </summary>
        public int IncomingBufferSize = 64;

        /// <summary>
        /// Maximum tranimission unit (MTU). To support every router keep it at its default value = 1400
        /// </summary>
        public int MaximumTransmissionUnit = 1400;

        /// <summary>
        /// The allocated size of the outgoing message buffer. If the buffer size exceeds it will resize the array which is performance-heavy.
        /// </summary>
        public int OutgoingBufferSize = 64;

        /// <summary>
        /// The amount of allocated incoming messages in the message pool. NOTICE: This number must be power of two.
        /// </summary>
        public int IncomingMessagePoolSize = 32;

        /// <summary>
        /// The amount of allocated outgoing messages in the message pool. NOTICE: This number must be power of two.
        /// </summary>
        public int OutgoingMessagePoolSize = 32;

        /// <summary>
        /// The amount of allocated connections in the connection pool. NOTICE: This number must be power of two.
        /// </summary>
        public int ConnectionPoolSize = 4;

        /// <summary>
        /// The number of channels to allocate for any default connection.
        /// </summary>
        public byte DefaultChannelAmount = 4;

        /// <summary>
        /// The IPEndPoint for a hosted server. For example, a NAT Relay server.
        /// </summary>
        public IPEndPoint ServerEndPoint;






        public NetConfiguration(uint appIdentification)
        {
            AppIdentification = appIdentification;
            LocalAddress = IPAddress.Loopback.GetAddressBytes();
        }

    }
}
