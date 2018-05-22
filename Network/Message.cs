﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public abstract unsafe class Message : IDisposable
    {
        public ENet.ENetPacket* Packet;
        public DeliveryMethod DeliveryMethod;
        internal byte[] Data = new byte[4];


        internal int BitLocation = 0;
        internal int BitLength = 0;

        public void Dispose()
        {
            Packet = null;
            Data = null;
            ENet.DestroyPacket(Packet);
        }


    }
}
