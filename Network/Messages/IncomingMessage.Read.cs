using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe partial class IncomingMessage
    {
        public bool ReadBool()
        {
            // Find the right index of the byte array
            int index = BitLocation >> 3;

            // Location of bit to read. Global readBitLocation minus the index of the array times 8 (size of byte)
            int bitToRead = BitLocation - (index * 8);

            // Lookup byte value in array, >> by bitToRead, to read value.

            // int value = Data[index] >> bitToRead;
            int value = Data[index] >> bitToRead;

            // Increase bit read location by 1 bit
            BitLocation++;

            // Applying a mask equals to 1 which in binary would be:         00000001
            // For example if value is 33 it has the binary code of          00100001
            // The AND operator multiplies each bit from above with below:   00000001 <-- Our value left is 1 == true for the boolean
            return (value & 1) == 1;   // Simple condition to return  a bool instead of a byte. Returning true if the result == 1.    
        }

        public byte ReadByte()
        {
            int index = BitLocation >> 3;

            int bitPosInByte = BitLocation - (index * 8);

            //Move Bit reader 8 bits = 1 byte
            BitLocation += 8;

            if (bitPosInByte == 0)
            {
                return Data[index];
            }

            int availableBits = 8 - bitPosInByte;
            // mask away unused bits lower than (right of) relevant bits in first byte
            byte firstByteResult = (byte)(Data[index] >> bitPosInByte);

            byte secondByteResult = Data[index + 1];

            // mask away unused bits (Bits that may be for another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);

            // Using an OR bit operator | we can add the two binary numbers from each byte  Example: 00000011
            // For example if value is 33 it has the binary code of                                  11001000
            // The OR would give following result:                                                   11001011 
            return (byte)(firstByteResult | secondByteResult << availableBits);
        }

        public ushort ReadUInt16()
        {   
            int result;

            int index = BitLocation >> 3;

            int bitPosInByte = BitLocation - (index * 8);

            BitLocation += 16;

            if (bitPosInByte == 0)
            {
                result = Data[index];
                result |= (Data[index + 1] << 8);
                return (ushort)result;
            }

            int availableBits = 8 - bitPosInByte;
            // mask away unused bits lower than (right of) relevant bits in first byte
            byte firstByteResult = (byte)(Data[index] >> bitPosInByte);
            index++;

            byte secondByteResult = Data[index];
            byte thirdByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte fourthByteResult = Data[index];

            // mask away unused bits (Bits that may be for another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);
            fourthByteResult &= (byte)(255 >> availableBits);

            return (ushort)(firstByteResult | secondByteResult << availableBits | (thirdByteResult | fourthByteResult << availableBits) << 8);      
        }

        /*
        public ulong ReadUInt64()
        {
            int index = BitLocation >> 3;

            int bitPosInByte = BitLocation - (index * 8);

            BitLocation += 64;

            if (bitPosInByte == 0)
            {
                return (ulong)(Data[index] | Data[index + 1] << 8 | Data[index + 2] << 16 | Data[index + 3] << 24 | Data[index + 4] << 32 | Data[index + 5] << 40 | Data[index + 6] << 48 
                    | Data[index + 7] << 56);
            }

            int availableBits = 8 - bitPosInByte;
            // mask away unused bits lower than (right of) relevant bits in first byte

            byte firstByteResult = (byte)(Data[index] >> bitPosInByte);
            index++;

            byte secondByteResult = Data[index];
            byte thirdByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte fourthByteResult = Data[index];
            byte fifthByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte sixthByteResult = Data[index];
            byte seventhByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte eighthByteResult = Data[index];
            byte ninethByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte tenthByteResult = Data[index];
            byte eleventhByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte twelvethByteResult = Data[index];
            byte thirteenByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte fourteenthByteResult = (Data[index]);
            byte fifthteenthByteResult = (byte)(Data[index] >> bitPosInByte);

            // mask away unused bits (Bits that are from another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);
            fourthByteResult &= (byte)(255 >> availableBits);
            sixthByteResult &= (byte)(255 >> availableBits);
            eighthByteResult &= (byte)(255 >> availableBits);
            tenthByteResult &= (byte)(255 >> availableBits);
            twelvethByteResult &= (byte)(255 >> availableBits);
            fourteenthByteResult &= (byte)(255 >> availableBits);


            return (ulong)((firstByteResult | secondByteResult << availableBits) | (thirdByteResult | fourthByteResult << availableBits) << 8 |
                (fifthByteResult | sixthByteResult << availableBits) << 16 | (seventhByteResult | eighthByteResult << availableBits) << 24 | (ninethByteResult | tenthByteResult << availableBits) << 32 | (eleventhByteResult | twelvethByteResult << availableBits) << 40 | (thirteenByteResult | fourteenthByteResult << availableBits) << 48 | fifthteenthByteResult  << 56);

        }
        */

        public ulong ReadUInt64()
        {
            ulong first = ReadUInt32();
            ulong second = ReadUInt32();

            return first + (second << 32);
        }

        public uint ReadUInt32()
        {
            int index = BitLocation >> 3;

            int bitPosInByte = BitLocation - (index * 8);

            BitLocation += 32;

            if (bitPosInByte == 0)
            {
                return (uint)(Data[index] | Data[index + 1] << 8 | Data[index + 2] << 16 | Data[index + 3] << 24);
            }

            int availableBits = 8 - bitPosInByte;
            // mask away unused bits lower than (right of) relevant bits in first byte
            byte firstByteResult = (byte)(Data[index] >> bitPosInByte);
            index++;

            byte secondByteResult = Data[index];
            byte thirdByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte fourthByteResult = Data[index];
            byte fifthByteResult = (byte)(Data[index] >> bitPosInByte);

            index++;
            byte sixthByteResult = Data[index];
            byte seventhByteResult = (byte)(Data[index] >> bitPosInByte);

            byte EightByteResult = (Data[index + 1]);

            // mask away unused bits (Bits that are from another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);
            fourthByteResult &= (byte)(255 >> availableBits);
            sixthByteResult &= (byte)(255 >> availableBits);

            return (uint)((firstByteResult | secondByteResult << availableBits) | (thirdByteResult | fourthByteResult << availableBits) << 8 | 
                (fifthByteResult | sixthByteResult << availableBits) << 16 | (seventhByteResult | EightByteResult << availableBits) << 24);     
        }


        /// <summary>
		/// Reads a string written using Write(string)
		/// </summary>
		public string ReadString()
        {
            int byteLen = ReadUInt16();

            if (byteLen <= 0)
                return string.Empty;

            if ((ulong)(BitLength - BitLocation) < ((ulong)byteLen * 8))
            {
				BitLocation = BitLength;
				return null; // unfortunate; but we need to protect against DDOS

            }

            if ((BitLocation & 7) == 0)
            {
                // read directly
                string retval = Encoding.UTF8.GetString(Data, BitLocation >> 3, byteLen);
                BitLocation += (8 * byteLen);
                return retval;
            }
            // Min 8 to hold anything but strings. Increase it if readed strings usally don't fit inside the buffer
            if (byteLen <= 64)
            {
                //    byte[] buffer = (byte[])Interlocked.Exchange(ref s_buffer, null) ?? new byte[c_bufferSize];

                int index = BitLocation >> 3;
                int startReadAtIndex = BitLocation - (index * 8); // (readBitOffset % 8);

                if (startReadAtIndex == 0)
                {
                    Buffer.BlockCopy(Data, index, Data, 0, byteLen);
                    return string.Empty;
                }

                int secondPartLen = 8 - startReadAtIndex;
                int secondMask = 255 >> secondPartLen;

                int byteOffset = 0;

                for (int i = 0; i < byteLen; i++)
                {
                    // mask away unused bits lower than (right of) relevant bits in byte
                    int b = Data[index] >> startReadAtIndex;

                    index++;

                    // mask away unused bits higher than (left of) relevant bits in second byte
                    int second = Data[index] & secondMask;

                    Data[byteOffset++] = (byte)(b | (second << secondPartLen));
                }

                string retval = Encoding.UTF8.GetString(Data, 0, byteLen);
    
                BitLocation += (8 * byteLen);
                return retval;
            }
            else
            {
                byte[] bytes = new byte[byteLen];
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }

        public IPEndPoint ReadIPEndPoint()
        {
            /*byte len = ReadByte();
            byte[] addressBytes = new byte[len];

            for (int i = 0; i < len; i++)
                addressBytes[i] = ReadByte();
                */

            byte[] addressBytes = { ReadByte(), ReadByte(), ReadByte(), ReadByte() };


            int port = ReadUInt16();


            return new IPEndPoint(new IPAddress(addressBytes), port);
        }


        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        public float ReadFloat()
        {
            NetUtilities.ReadSingle.Integer = ReadUInt32();
            return NetUtilities.ReadSingle.Value;
        }

    }
}
