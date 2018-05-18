﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public static unsafe partial class ENet
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ENetAddress
        {
            public uint Host;
            public ushort Port;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENetHost
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENetChannel
        {
            public ushort outgoingReliableSequenceNumber;
            public ushort outgoingUnreliableSequenceNumber;
            public ushort usedReliableWindows;
            public ushort reliableWindows;
            public ushort incomingReliableSequenceNumber;
            public ushort incomingUnreliableSequenceNumber;
            public ENetList* incomingReliableCommands;
            public ENetList* incomingUnreliableCommands;
        }

        public struct ENetCompressor
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void compress_cb(IntPtr context, IntPtr inBuffers, IntPtr inBufferCount, IntPtr inLimit, IntPtr outData, IntPtr outLimit);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void decompress_cb(IntPtr context, IntPtr inData, IntPtr inLimit, IntPtr outData, IntPtr outLimit);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void destroy_cb(IntPtr context);

            public IntPtr context;
            public IntPtr compress, decompress, destroy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENetEvent
        {
            public MessageType type;
            public ENetPeer* peer;
            public byte channelID;
            public uint data;
            public ENetPacket* packet;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct ENetListNode
        {
            public ENetListNode* next, previous;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct ENetList
        {
            public ENetListNode* sentinel;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENetPacket
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void freeCallback_cb(IntPtr* packet);

            public IntPtr referenceCount;
            public DeliveryMethod flags;
            public IntPtr data;
            public IntPtr dataLength;
            public IntPtr freeCallback;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct ENetCallbacks
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr malloc_cb(IntPtr size);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void free_cb(IntPtr memory);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void no_memory_cb();

            public IntPtr malloc, free, no_memory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENetPeer
        {
            public ENetListNode dispatchList;
            public ENetHost* host;
            public ushort outgoingPeerID;
            public ushort incomingPeerID;
            public uint connectID;
            public byte outgoingSessionID;
            public byte incomingSessionID;
            public ENetAddress address;
            public IntPtr data;
            public PeerState state;
            public ENetChannel* channels;
            public IntPtr channelcount;
            public uint incomingBandwidth;
            public uint outgoingBandwidth;
            public uint incomingBandwidthThrottleEpoch;
            public uint outgoingBandwidthThrottleEpoch;
            public uint incomingDataTotal;
            public uint outgoingDataTotal;
            public uint lastSendTime;
            public uint lastReceiveTime;
            public uint nextTimeout;
            public uint earliestTimeout;
            public uint packetLossEpoch;
            public uint packetsSent;
            public uint packetsLost;
            public uint packetLoss;
            public uint packetLossVariance;
            public uint packetThrottle;
            public uint packetThrottleLimit;
            public uint packetThrottleCounter;
            public uint packetThrottleEpoch;
            public uint packetThrottleAcceleration;
            public uint packetThrottleDeceleration;
            public uint packetThrottleInterval;
            public uint pingInterval;
            public uint timeoutLimit;
            public uint timeoutMinimum;
            public uint timeoutMaximum;
            public uint lastRoundTripTime;
            public uint lowestRoundTripTime;
            public uint lastRoundTripTimeVariance;
            public uint highestRoundTripTimeVariance;
            public uint roundTripTime;
            public uint roundTripTimeVariance;
            public uint mtu;
            public uint windowSize;
            public uint reliableDataInTransit;
            public ushort outgoingReliableSequenceNumber;
            public ENetList* acknowledgements;
            public ENetList* sentReliableCommands;
            public ENetList* sentUnreliableCommands;
            public ENetList* outgoingReliableCommands;
            public ENetList* outgoingUnreliableCommands;
            public ENetList* dispatchedCommands;
            public int needsDispatch;
            public ushort incomingUnsequencedGroup;
            public ushort outgoingUnsequencedGroup;
            public uint unsequencedWindow;
            public uint eventData;
        }
    }
}
