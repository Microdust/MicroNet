using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{

    /// <summary>
    /// Specifies outgoing packet transmittion behavior.
    /// </summary>
    [Flags]
    public enum DeliveryMethod
    {
        /// <summary>
        ///  Unreliable sequenced == Discards the lower sequence number packet if a packet with a higher sequence number has already been delivered.
        /// </summary>
        None = 0,

        /// <summary>
        /// Send the packet reliably.
        /// </summary>
        Reliable = 1 << 0,

        /// <summary>
        /// Allow the packet to arrive out-of-order.
        /// </summary>
        Unsequenced = 1 << 1,

        /// <summary>
        /// Let the application, not ENet, handle memory allocation for the packet.
        /// </summary>
        NoAllocate = 1 << 2,

        /// <summary>
        /// Even if an unreliable packet is larger than the MTU
        /// and requires fragmentation, send it unreliably.
        /// </summary>
        UnreliableFragment = 1 << 3
    }
}
