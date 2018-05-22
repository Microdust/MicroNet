using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
    }
}
