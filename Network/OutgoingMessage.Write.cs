using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{ 
    public unsafe partial class OutgoingMessage
    {

        private void EnsureBuffer()
        {

        }



        /// <summary>
        /// Write a string to the message
        /// </summary>
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteUInt32(0);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int bytesToSend = bytes.Length;

            WriteUInt32((uint)bytes.Length);

            int byteLenght = BitLength + 8 + (bytes.Length * 8) >> 3;
            if (bytes.Length < byteLenght)
            {
                Array.Resize<byte>(ref Data, byteLenght);
            }

            
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
        public void WriteBool(bool value)
        {
            int byteLenght = BitLength + 1 >> 3;
            if (Data.Length < byteLenght)
            {
                Array.Resize<byte>(ref Data, byteLenght);
            }
                

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
        public void WriteByte(byte value)
        {
            int byteLenght = BitLength + 8 >> 3;
            if (Data.Length < byteLenght)
            {
                Array.Resize<byte>(ref Data, byteLenght);
            }


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

        // Better
        public void WriteFloat(float value)
        {
            int byteLenght = BitLength + 32 >> 3;
            if (Data.Length < byteLenght)
            {
                Debug.Log("Exceed");
                Array.Resize<byte>(ref Data, byteLenght);
            }

            NetUtilities.Single.Value = value;
            WriteUInt32(NetUtilities.Single.Integer);
        }

        // Better
        public void WriteUInt32(uint value)
        {
            int byteLenght = BitLength + 32 >> 3;
            if (Data.Length < byteLenght)
            {
                Debug.Log("Exceed");
                Array.Resize<byte>(ref Data, byteLenght);
            }

#if BIGENDIAN
			// reorder bytes
			source = ((source & 0xff000000) >> 24) |
				((source & 0x00ff0000) >> 8) |
				((source & 0x0000ff00) << 8) |
				((source & 0x000000ff) << 24);
#endif

            WriteByte((byte)value);

            WriteByte((byte)(value >> 8));

            WriteByte((byte)(value >> 16));

            WriteByte((byte)(value >> 24));
        }


        /// <summary>
        /// Writes an unsigned 16 bit integer
        /// </summary>
        public void WriteUInt16(ushort value)
        {
            int byteLenght = BitLength + 16 >> 3;
            if (Data.Length < byteLenght)
            {
                Debug.Log("Exceed");
                Array.Resize<byte>(ref Data, byteLenght);
            }

#if BIGENDIAN
			uint intSource = value;
			intSource = ((intSource & 0x0000ff00) >> 8) | ((intSource & 0x000000ff) << 8);
			value = (ushort)intSource;
#endif

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
        }

    }
}
