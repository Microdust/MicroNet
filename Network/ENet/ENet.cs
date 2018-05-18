using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public static unsafe partial class ENet
    {
        private  const string DLL = "ENet.dll";

        public const int PEER_PACKET_THROTTLE_SCALE = 32;
        public const int PEER_PACKET_THROTTLE_ACCELERATION = 2;
        public const int PEER_PACKET_THROTTLE_DECELERATION = 2;
        public const int PEER_PACKET_THROTTLE_INTERVAL = 5000;
        public const int PROTOCOL_MINIMUM_CHANNEL_COUNT = 0x01;
        public const int PROTOCOL_MAXIMUM_CHANNEL_COUNT = 0xff;
        public const int PROTOCOL_MAXIMUM_PEER_ID = 0xfff;
        public const uint VERSION = (1 << 16) | (3 << 8) | (1);
        public const uint HOST_ANY = 0;
        public const uint HOST_BROADCAST = 0xffffffff;


        #region ENet Eseentials

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_initialize")]
        public static extern int Initialize();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_deinitialize")]
        public static extern void Deinitialize();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_initialize_with_callbacks")]
        public static extern int InitializeWithCallback(uint version, ref ENetCallbacks inits);
        #endregion
        #region Address
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_set_host")]
        public static extern int AddressSetHost(ref ENetAddress address, byte* hostName);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_set_host")]
        public static extern int AddressSetHost(ref ENetAddress address, byte[] hostName);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_get_host")]
        public static extern int AddressGetHost(ref ENetAddress address, byte* hostName, IntPtr nameLength);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_get_host")]
        public static extern int AddressGetHost(ref ENetAddress address, byte[] hostName, IntPtr nameLength);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_get_host_ip")]
        public static extern int AddressGetHostIp(ref ENetAddress address, byte* hostIP, IntPtr ipLength);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_address_get_host_ip")]
        public static extern int AddressGetHostIp(ref ENetAddress address, byte[] hostIP, IntPtr ipLength);
        #endregion
        #region Hosting
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_compress_with_range_coder")]
        public static extern int HostCompressRange(ENetHost* host);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_create")]
        public static extern ENetHost* CreateHost(ENetAddress* address,
                                                        IntPtr peerLimit, IntPtr channelLimit, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_create")]
        public static extern ENetHost* CreateHost(ref ENetAddress address,
                                                        IntPtr peerLimit, IntPtr channelLimit, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_destroy")]
        public static extern void DestroyHost(ENetHost* host);
 
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_connect")]
        public static extern ENetPeer* ConnectHost(ENetHost* host, ref ENetAddress address, IntPtr channelCount, uint data);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_broadcast")]
        public static extern void BroadcastHost(ENetHost* host, byte channelID, ENetPacket* packet);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_compress")]
        public static extern void CompressHost(ENetHost* host, ENetCompressor* compressor);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_channel_limit")]
        public static extern void ChannelLimitHost(ENetHost* host, IntPtr channelLimit);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_bandwidth_limit")]
        public static extern void BandwidthLimitHost(ENetHost* host, uint incomingBandwidth, uint outgoingBandwidth);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_flush")]
        public static extern void FlushHost(ENetHost* host);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_check_events")]
        public static extern int CheckEventsHost(ENetHost* host, out ENetEvent @event);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_service")]
        public static extern int ServiceHost(ENetHost* host, ENetEvent* @event, uint timeout);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_host_service")]
        public static extern int ServiceHost(ENetHost* host, out ENetEvent @event, uint timeout);

        #endregion
        #region Utility

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_time_get")]
        public static extern uint GetTime();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_time_set")]
        public static extern void SetTime(uint newTimeBase);

        #endregion
        #region Packet

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_packet_create")]
        public static extern ENetPacket* CreatePacket(void* data, IntPtr dataLength, DeliveryMethod flags);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_packet_destroy")]
        public static extern void DestroyPacket(ENetPacket* packet);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_packet_resize")]
        public static extern int ResizePacket(ENetPacket* packet, IntPtr dataLength);

        #endregion
        #region Peer

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_throttle_configure")]
        public static extern void ThrottleConfigurePeer(ENetPeer* peer, uint interval, uint acceleration, uint deceleration);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_send")]
        public static extern int SendPeer(ENetPeer* peer, byte channelID, ENetPacket* packet);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_receive")]
        public static extern ENetPacket* ReceivePeer(ENetPeer* peer, out byte channelID);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_reset")]
        public static extern void ResetPeer(ENetPeer* peer);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_ping")]
        public static extern void PingPeer(ENetPeer* peer);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_disconnect_now")]
        public static extern void DisconnectPeerNow(ENetPeer* peer, uint data);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_disconnect")]
        public static extern void DisconnectPeer(ENetPeer* peer, uint data);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "enet_peer_disconnect_later")]
        public static extern void DisconnectPeerLater(ENetPeer* peer, uint data);

        #endregion

    }
}
