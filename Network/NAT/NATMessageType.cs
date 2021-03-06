﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public static class NATMessageType
    { 
    
        public const int ACK = 16;

        public const int INITIATE_HOST = 32;

        public const int REQUEST_INTRODUCTION = 64;

        public const int INTRODUCTION = 128;

  //      public const int GET_HOST_LIST = 256;

        public const int NAT_PUNCHTHROUGH = 512;

        public const int NAT_PUNCH_SUCCESS = 1024;

        public const int REQUEST_HOST_LIST = 2048;

    }
}
