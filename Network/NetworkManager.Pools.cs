using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe partial class NetworkManager
    {
        private ConcurrentBag<IncomingMessage> incomingPool;
        private Stack<RemoteConnection> connectionPool;

        public void InitializePools()
        {
            MessagePool.InitializOutgoingPool(config.OutgoingMessagePoolSize, config.OutgoingBufferSize);
            incomingPool = new ConcurrentBag<IncomingMessage>();
            connectionPool = new Stack<RemoteConnection>(config.ConnectionPoolSize);

            for (int i = 0; i < config.OutgoingMessagePoolSize; i++)
            {
                incomingPool.Add(new IncomingMessage(config.IncomingBufferSize));
            }

            for (int i = 0; i < config.ConnectionPoolSize; i++)
            {
                connectionPool.Push(new RemoteConnection());
            }

        }

        private void Recycle(IncomingMessage msg)
        {
            msg.Reset();
            incomingPool.Add(msg);
        }

        private void Recycle(RemoteConnection remote)
        {
            remote.Reset();
            connectionPool.Push(remote);
        }

        private IncomingMessage GetIncomingMessage()
        {
            incomingPool.TryTake(out IncomingMessage msg);
            return msg;
        }

        private RemoteConnection GetRemoteConnection(ENet.Peer* peer)
        {
            RemoteConnection remote = connectionPool.Pop();
            remote.Initialize(peer);
            return remote;
        }

        /*
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
            sizePool = messagePool.Length;

            for (int i = 0; i < sizePool; i++)
            {
                messagePool[i] = new IncomingMessage(config.MessageBufferSize);
            }

        }

        public IncomingMessage GetIncomingMessage()
        {
            if (sizePool == -1)
                return new IncomingMessage(config.MessageBufferSize);

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
        */
    }
}
