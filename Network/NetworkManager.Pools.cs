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

        // RemoteConnectionPool
        private RemoteConnection[] pool;
        private int connectionPoolSize;          

        public void InitializePools()
        {
            MessagePool.InitializOutgoingPool(config.OutgoingMessagePoolSize, config.OutgoingBufferSize);
            incomingPool = new ConcurrentBag<IncomingMessage>();
            

            for (int i = 0; i < config.OutgoingMessagePoolSize; i++)
            {
                incomingPool.Add(new IncomingMessage(config.IncomingBufferSize));
            }

            connectionPoolSize = config.ConnectionPoolSize;
            pool = new RemoteConnection[config.ConnectionPoolSize];
 
            for (int i = 0; i < connectionPoolSize; i++)
            {
                pool[i] = new RemoteConnection();
            }

           
        }

        internal RemoteConnection CreateRemoteConnection(ENet.Peer* peer)
        {
            if (connectionPoolSize == 0)
                return new RemoteConnection(peer);

            RemoteConnection remote = pool[--connectionPoolSize];
            pool[connectionPoolSize] = null;     // Free memory quicker.

            remote.Initialize(peer);
            
            return remote;
        }

        internal void Recycle(RemoteConnection remote)
        {
            if (connectionPoolSize == pool.Length)
            {
                RemoteConnection[] newArray = new RemoteConnection[2 * pool.Length];
                Array.Copy(pool, 0, newArray, 0, connectionPoolSize);
                pool = newArray;
                Debug.Error(config.Name, ": NetworkManager.Pools - Recycled more remotes than space for!");
            }

            remote.Peer = null;

            pool[connectionPoolSize++] = remote;
        }

        private void Recycle(IncomingMessage msg)
        {
            msg.Reset();
            incomingPool.Add(msg);
        }


        private IncomingMessage GetIncomingMessage()
        {
            incomingPool.TryTake(out IncomingMessage msg);
            return msg;
        }
     
    }
}
