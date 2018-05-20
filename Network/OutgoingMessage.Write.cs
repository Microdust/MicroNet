using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{ 
    public unsafe partial class OutgoingMessage
    {
        public void Write()
        {

        }


        /// <summary>
        /// Write a boolean to the outgoing message.
        /// </summary>
        public void WriteBool(bool value)
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
            int p = BitLength >> 3;

            // Inserting the new bit					
            Data[p] = (byte)((Data[p]) | (boolean << occupiedBits));

            // Increase the BitLength of the message by 1 == boolean size
            BitLength++;
        }


        public void WriteByte(bool value)
        {
            byte boolean = *((byte*)(&value));
            // Bitwise modulus of 8, possible due to substraction of 1 ... (stackoverflow)
            // Our current bit length modulus of 8 (size of byte) == how many bits in the byte is already in use.
            int occupiedBits = BitLength & 7;

            // How many bits are available in the byte
                int availableBits = 8 - occupiedBits;

          //   How many bits left after our addition,
                  int bitsLeft = availableBits - 1;

            // Calculate the index of the array based on the current BitLength,
            // Shifts bits to the right. The number to the left of the operator
            // is shifted the number of places specified by the number to the right. 
            // Each shift to the right halves the number, therefore each right shift divides the original number by 2. 
              int p = BitLength >> 3;
              
              int mask = (255 >> availableBits) | (255 << (8 - bitsLeft));

            // Masking lower and upper bits         // Inserting the new bit					
               Data[p] = (byte)((Data[p] & mask) | (boolean << occupiedBits));

            // Increase the BitLength of the message by 8 (size of byte)
            BitLength += 8;
        }

    }
}
