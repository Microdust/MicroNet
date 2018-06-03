using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public partial class NetworkManager
    {
        private IncomingMessage[] messagePool;
        private int headPool;       // First valid element in the queue
        private int tailPool;       // Last valid element in the queue
        private int sizePool;       // Number of elements.
        private readonly int growFactorPool = 2;

        public void InitializePools(int capacity)
        {
            if ((capacity & (capacity - 1)) == 0)
            {
                messagePool = new IncomingMessage[capacity];
            }
            else
            {
                Debug.Error("MessagePool capacity value is not power of two. Defaults to 16");
                messagePool = new IncomingMessage[16];
            }

            headPool = 0;
            tailPool = 0;
            sizePool = 0;

            for (int i = 0; i < messagePool.Length; i++)
            {
                messagePool[i] = new IncomingMessage(config.MinimumByteSize);
            }

        }

        public IncomingMessage GetIncomingMessage()
        {
            if (sizePool == 0)
                return new IncomingMessage(config.MinimumByteSize);

            IncomingMessage removed = messagePool[headPool];
            messagePool[headPool] = null;
            headPool = (headPool + 1) & (messagePool.Length - 1); // power of two
            sizePool--;
            return removed;
        }

        /// <summary>
        /// Recycle an 'IncomingMessage' by returning it to the internal message pool
        /// </summary>
        public void Recycle(IncomingMessage msg)
        {
            if (sizePool == messagePool.Length)
            {
                int newLength = messagePool.Length * growFactorPool;
                IncomingMessage[] tempArray = new IncomingMessage[newLength];
                if (sizePool > 0)
                {
                    if (headPool < tailPool)
                    {
                        Array.Copy(messagePool, headPool, tempArray, 0, sizePool);
                    }
                    else
                    {
                        Array.Copy(messagePool, headPool, tempArray, 0, messagePool.Length - headPool);
                        Array.Copy(messagePool, 0, tempArray, messagePool.Length - headPool, tailPool);
                    }
                }
                messagePool = tempArray;
                headPool = 0;
                tailPool = (sizePool == newLength) ? 0 : sizePool;
            }

            msg.Reset();

            messagePool[tailPool] = msg;
            tailPool = (tailPool + 1) & (messagePool.Length - 1); // Power of two
            sizePool++;

        }

    }
}
