﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public static class MessagePool
    {
        private static OutgoingMessage[] pool;
        private static int size;
        private static int defaultBufferSize;

        internal static void InitializOutgoingPool(int capacity, int bufferSize)
        {
            size = capacity;
            defaultBufferSize = bufferSize;
            pool = new OutgoingMessage[capacity];
           
            for (int i = 0; i < capacity; i++)
            {
                pool[i] = new OutgoingMessage(bufferSize);
            }

        }

        public static OutgoingMessage CreateMessage()
        {
            if (size == 0)
                return new OutgoingMessage(defaultBufferSize);

            OutgoingMessage msg = pool[--size];
            pool[size] = null;     // Free memory quicker.

            return msg;
        }

        public static void Recycle(OutgoingMessage msg)
        {
            if (size == pool.Length)
            {
                OutgoingMessage[] newArray = new OutgoingMessage[2 * pool.Length];
                Array.Copy(pool, 0, newArray, 0, size);
                pool = newArray;
            }

            msg.Recycle();

            pool[size++] = msg;
        }




        /*
    private static OutgoingMessage[] messagePool;
    private static int headPool;       // First valid element in the queue
    private static int tailPool;       // Last valid element in the queue
    private static int sizePool;       // Number of elements.
    private static readonly int growFactorPool = 2;

    private static int minSize;

    public static void InitializePool(int capacity, int minimumByte)
    {
        if ((capacity & (capacity - 1)) == 0)
        {
            messagePool = new OutgoingMessage[capacity];
        }
        else
        {
            Debug.Error("MessagePool capacity value is not power of two. Defaults to 16");
            messagePool = new OutgoingMessage[16];
        }
        minSize = minimumByte;

        headPool = 0;
        tailPool = 0;
        sizePool = messagePool.Length;

        for (int i = 0; i < sizePool; i++)
        {
            messagePool[i] = new OutgoingMessage(minSize);
        }

    }

    public static OutgoingMessage CreateMessage()
    {
        if (sizePool == 0)
            return new OutgoingMessage(minSize);

        OutgoingMessage removed = messagePool[headPool];
        messagePool[headPool] = null;
        headPool = (headPool + 1) & (messagePool.Length - 1); // power of two
        sizePool--;
        return removed;
    }

    /// <summary>
    /// Recycle an 'IncomingMessage' by returning it to the internal message pool
    /// </summary>
    public static void Recycle(OutgoingMessage msg)
    {
        if (sizePool == messagePool.Length)
        {
            int newLength = messagePool.Length * growFactorPool;
            OutgoingMessage[] tempArray = new OutgoingMessage[newLength];
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

        msg.Recycle();

        messagePool[tailPool] = msg;
        tailPool = (tailPool + 1) & (messagePool.Length - 1); // Power of two
        sizePool++;

    }
    */

    }
}
