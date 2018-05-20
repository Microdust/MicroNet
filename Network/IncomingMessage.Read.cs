using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public partial class IncomingMessage
    {

        public bool ReadBool()
        {
            // Find the right index of the byte array
            int byteArrayIndex = BitLocation >> 3;

            // Location of bit to read. Global readBitLocation minus the index of the array times 8 (size of byte)
            int bitToRead = BitLocation - (byteArrayIndex * 8);


            // Lookup byte value in array, >> by bitToRead, to read value.
            int value = Data[byteArrayIndex] >> bitToRead;

            // Increase bit read location by 1 bit
            BitLocation++;

            // Applying a mask equals to 1 which in binary would be:         00000001
            // For example if value is 33 it has the binary code of          00100001
            // The AND operator multiplies each bit from above with below:   00000001 <-- Our value left is 1 == true for the boolean
            return (value & 1) == 1;   // Simple condition to return  a bool instead of a byte. Returning true if the result == 1.      
        }
    }
}
