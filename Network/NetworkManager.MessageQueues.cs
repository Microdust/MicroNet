using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public partial class NetworkManager
    {
        private IncomingMessage[] messageQueue;
        private IncomingMessage dequeuedIncomingMessage;

        private ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();

        private int head;       // First valid element in the queue
        private int tail;       // Last valid element in the queue
        private int size;       // Number of elements.
        private readonly int growFactor = 2;
        

        /// <summary>
        /// Capacity must be a power of two value
        /// </summary>
        public void InitializeQueues(int capacity)
        {
            if ((capacity & (capacity -1)) == 0)
            {
                messageQueue = new IncomingMessage[capacity];
            }
            else
            {
                Debug.Error("MessagePool capacity value is not power of two. Defaults to 16");
                messageQueue = new IncomingMessage[16];
            }
           
            head = 0;
            tail = 0;
            size = 0;

        }

        /// <summary>
        /// Add a message 
        /// </summary>
        public void IncomingEnqueue(IncomingMessage msg)
        {
            slimLock.EnterWriteLock();
            try
            {
                if (size == messageQueue.Length)
                {
                    int newLength = messageQueue.Length * growFactor;
                    IncomingMessage[] tempArray = new IncomingMessage[newLength];
                    if (size > 0)
                    {
                        if (head < tail)
                        {
                            Array.Copy(messageQueue, head, tempArray, 0, size);
                        }
                        else
                        {
                            Array.Copy(messageQueue, head, tempArray, 0, messageQueue.Length - head);
                            Array.Copy(messageQueue, 0, tempArray, messageQueue.Length - head, tail);
                        }
                    }
                    messageQueue = tempArray;
                    head = 0;
                    tail = (size == newLength) ? 0 : size;
                }

                messageQueue[tail] = msg;
                tail = (tail + 1) & (messageQueue.Length - 1); // Power of two                    
                size++;
            }
            finally
            {
                slimLock.ExitWriteLock();
            }

            

            
        }

        /// <summary>
        /// Get a pending IncomingMessage from the queue
        /// </summary>
        public IncomingMessage IncomingDequeue()
        {
            if (size == 0)
                return null;

            slimLock.EnterWriteLock();
            try
            {
                dequeuedIncomingMessage = messageQueue[head];
                messageQueue[head] = null;
                head = (head + 1) & (messageQueue.Length - 1); // power of two
                size--;
            }
            finally
            {
                slimLock.ExitWriteLock();
            }

            return dequeuedIncomingMessage;

        }

        public void IncomingClear()
        {
            if (head < tail)
            {
                Array.Clear(messageQueue, head, size);
            }
            else
            {
                Array.Clear(messageQueue, head, messageQueue.Length - head);
            }
            head = 0;
            tail = 0;
            size = 0;
        }


    }
}
