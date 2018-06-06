﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            // mask away unused bits (Bits that may be for another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);


            result = firstByteResult | secondByteResult << availableBits | thirdByteResult << 8;


#if BIGENDIAN
			// reorder bytes
			uint retVal = returnValue;
			retVal = ((retVal & 0x0000ff00) >> 8) | ((retVal & 0x000000ff) << 8);
			return (ushort)retVal;
#else
            return (ushort)result;
#endif        
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

            // mask away unused bits (Bits that may be for another read, in the same byte)
            secondByteResult &= (byte)(255 >> availableBits);
            fourthByteResult &= (byte)(255 >> availableBits);
            sixthByteResult &= (byte)(255 >> availableBits);

            uint returnValue;

            returnValue = (uint)((firstByteResult | secondByteResult << availableBits) | (thirdByteResult | fourthByteResult << availableBits) << 8 | 
                (fifthByteResult | sixthByteResult << availableBits) << 16 | (seventhByteResult | EightByteResult << availableBits) << 24);


#if BIGENDIAN
			// reorder bytes
			return
				((returnValue & 0xff000000) >> 24) |
				((returnValue & 0x00ff0000) >> 8) |
				((returnValue & 0x0000ff00) << 8) |
				((returnValue & 0x000000ff) << 24);
#else
            return returnValue;
#endif           
          

        }


        public float ReadFloat()
        {
            uint integer;

            integer = ReadByte();

            integer |= (uint)(ReadByte() << 8);

            integer |= (uint)(ReadByte() << 16);

            integer |= (uint)(ReadByte() << 24);

#if BIGENDIAN
			// reorder bytes
			return
				((returnValue & 0xff000000) >> 24) |
				((returnValue & 0x00ff0000) >> 8) |
				((returnValue & 0x0000ff00) << 8) |
				((returnValue & 0x000000ff) << 24);
#else

            
            NetUtilities.ReadSingle.Integer = integer;
            return NetUtilities.ReadSingle.Value;
#endif
        }


      
        /// <summary>
		/// Reads a string written using Write(string)
		/// </summary>
		public string ReadString()
        {
            int byteLen = (int)ReadUInt32();

            if (byteLen <= 0)
                return string.Empty;

            if ((ulong)(BitLength - BitLocation) < ((ulong)byteLen * 8))
            {
                // not enough data
                Debug.Error("Overflow error");
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

    }
}
