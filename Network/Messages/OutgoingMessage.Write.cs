using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{ 
    public unsafe partial class OutgoingMessage
    {

        private void CheckAndBalance(int bits)
        {         
            ByteCount = BitLength + bits >> 3;
            if (Data.Length < ByteCount)
            {
                Debug.Log("Exceed");
                Array.Resize(ref Data, ByteCount);
            }
        }

        /// <summary>
        /// Write a string to the message
        /// </summary>
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                CheckAndBalance(16);
                WriteUInt16(0);
                return;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            
            CheckAndBalance(24 + (bytes.Length * 8));
            WriteUInt16((ushort)bytes.Length);

            int bytesToSend = bytes.Length;

            int index = BitLength >> 3;

            int occupiedBits = (BitLength % 8);

            if (occupiedBits == 0)
            {
                Buffer.BlockCopy(bytes, 0, Data, index, bytes.Length);
                BitLength += bytesToSend * 8;
                return;
            }

            int availableBits = 8 - occupiedBits;

            for (int i = 0; i < bytesToSend; i++)
            {
                byte src = bytes[i];

                // write last part of this byte
                Data[index] &= (byte)(255 >> availableBits); // clear before writing
                Data[index] |= (byte)(src << occupiedBits); // write first half

                index++;

                // write first part of next byte
                Data[index] &= (byte)(255 << occupiedBits); // clear before writing
                Data[index] |= (byte)(src >> availableBits); // write second half
            }
            BitLength += bytesToSend * 8;
        }




        /// <summary>
        /// Write a boolean to the outgoing message.
        /// </summary>
        private void WriteBool(bool value)
        {
            // Bit conversion from from bool to byte
            byte boolean = *((byte*)(&value));

            // Bitwise modulus of 8, possible due to substraction of 1 ... (stackoverflow)
            // Our current bit length modulus of 8 (size of byte) == how many bits in the byte is already in use.
            int occupiedBits = BitLength & 7;

            // Calculate the index of the array based on the current BitLength,
            // Shifts bits to the right. The number to the left of the operator
            // is shifted the number of places specified by the number to the right. 
            // Each shift to the right halves the number, therefore each right shift divides the original number by 2. 
            int index = BitLength >> 3;

            // Inserting the new bit					
            Data[index] = (byte)((Data[index]) | (boolean << occupiedBits));

            // Increase the BitLength of the message by 1 == boolean size

            BitLength++;
          
        }


        /// <summary>
        /// Write a byte to the outgoing message.
        /// </summary>
        private void WriteByte(byte value)
        {
            int occupiedBits = BitLength & 7;
            // How many bits are available in the byte
            int availableBits = 8 - occupiedBits;
            // How many bits left after our addition,
      //      int remainings =  8 - availableBits;

            // Calculate the index of the array based on the current BitLength,
            // Shifts bits to the right. The number to the left of the operator
            // is shifted the number of places specified by the number to the right. 
            // Each shift to the right halves the number, therefore each right shift divides the original number by 2. 
            int index = BitLength >> 3;

            BitLength += 8;

            // If there are 0 remaining bits afterwards, it all can fit into one byte!
            if (availableBits == 8)
            {
                Data[index] = (byte)(Data[index] | (value << occupiedBits));
                return;
            }

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));
        }


        private void WriteUInt32(uint value)
        {          
            int occupiedBits = BitLength & 7;

            int availableBits = 8 - occupiedBits;

            int index = BitLength >> 3;

            BitLength += 32;

            if (availableBits == 8)
            {
                Data[index] = (byte)(Data[index] | value);
                index++;
                Data[index] = (byte)(Data[index] | (value >> 8));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 16));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 24));
                return;
            }

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << (occupiedBits)));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

        }
        private void WriteUInt64(ulong value)
        {
            WriteUInt32((uint)value);
            WriteUInt32((uint)value >> 32);
        }

        /*
        public void WriteUInt64(ulong value)
        {
            int byteLenght = BitLength + 64 >> 3;
            if (Data.Length < byteLenght)
            {
                Debug.Log("Exceed");
                Array.Resize<byte>(ref Data, byteLenght);
            }

            int occupiedBits = BitLength & 7;

            int availableBits = 8 - occupiedBits;

            int index = BitLength >> 3;

            BitLength += 64;

            if (availableBits == 8)
            {
                Data[index] = (byte)(Data[index] | value);
                index++;
                Data[index] = (byte)(Data[index] | (value >> 8));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 16));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 24));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 32));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 40));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 48));
                index++;
                Data[index] = (byte)(Data[index] | (value >> 56));
                return;
            }

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << (occupiedBits)));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << (occupiedBits)));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

            value >>= 8;

            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));

        }
        */



        /// <summary>
        /// Writes an unsigned 16 bit integer
        /// </summary>
        private void WriteUInt16(ushort value)
        {
            int occupiedBits = BitLength & 7;

            int availableBits = 8 - occupiedBits;

            int index = BitLength >> 3;

            BitLength += 16;
            if (availableBits == 8)
            {
                Data[index] = (byte)(Data[index] | value);
                index++;
                Data[index] = (byte)(Data[index] | (value >> 8));               
                return;
            }
            
            Data[index] = (byte)(Data[index] | (value << occupiedBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> availableBits));
            index++;
            Data[index] = (byte)(Data[index] | (value >> (16 - occupiedBits)));
            
        }

        public void Write(IPEndPoint endPoint)
        {
            byte[] bytes = endPoint.Address.GetAddressBytes();
           // CheckAndBalance(16 + bytes.Length);
            CheckAndBalance(48);
           // Write((byte)bytes.Length);

           // for (int i = 0; i < bytes.Length; i++)
            WriteByte(bytes[0]);
            WriteByte(bytes[1]);
            WriteByte(bytes[2]);
            WriteByte(bytes[3]);


            Write((ushort)endPoint.Port);
        }


        private void WriteInternalFloat(float value)
        {
            NetUtilities.WriteSingle.Value = value;
            WriteUInt32(NetUtilities.WriteSingle.Integer);
        }

        public void Write(sbyte value)
        {
            CheckAndBalance(8);
            WriteByte((byte)value);
        }

        public void Write(byte value)
        {
            CheckAndBalance(8);
            WriteByte(value);
        }

        public void Write(short value)
        {
            CheckAndBalance(16);
            WriteUInt16((ushort)value);
        }

        public void Write(ushort value)
        {
            CheckAndBalance(16);
            WriteUInt16(value);
        }

        public void Write(int value)
        {
            CheckAndBalance(32);
            WriteUInt32((uint)value);
        }

        public void Write(uint value)
        {
            CheckAndBalance(32);
            WriteUInt32(value);
        }

        public void Write(long value)
        {
            CheckAndBalance(64);
            WriteUInt64((ulong)value);
        }

        public void Write(ulong value)
        {
            CheckAndBalance(64);
            WriteUInt64(value);
        }

        public void Write(bool value)
        {
            CheckAndBalance(1);
            WriteBool(value);
        }

        public void Write(float value)
        {
            CheckAndBalance(32);

            NetUtilities.WriteSingle.Value = value;
            WriteUInt32(NetUtilities.WriteSingle.Integer);
        }

    }
}
