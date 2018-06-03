using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    [StructLayout(LayoutKind.Explicit)]
    public struct NetSingle
    {
        [FieldOffset(0)]
        public float Value;

        [FieldOffset(0)]
        public uint Integer;

        public float GetFloat(uint integer)
        {
            Integer = integer;
            return Value;
        }

        public float GetUInt(float value)
        {
            Value = value;
            return Integer;
        }

    }

}
