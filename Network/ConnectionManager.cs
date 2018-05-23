using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroNet.Network
{
    public unsafe class ConnectionManager
    {
        private const int defaultCapacity = 2;

        private ENet.ENetPeer*[] connections;

        private int count = 0;

        private static readonly ENet.ENetPeer*[] clearArray = new ENet.ENetPeer*[0];

        public ConnectionManager()
        {
            connections = clearArray;
        }

        public ConnectionManager(int capacity)
        {
            if (capacity > 0)
            {
                connections = new ENet.ENetPeer*[capacity];
            }
            else
            {
                connections = clearArray;
            }
            
        }

        public int Capacity
        {
            get { return connections.Length; }
            set
            {
                if (value > 0)
                {
                    ENet.ENetPeer*[] newItems = new ENet.ENetPeer*[value];
                    Array.Copy(connections, 0, newItems, 0, count);
                    connections = newItems;
                }
                else
                {
                    connections = clearArray;
                }
            }
        }

        /// <summary>
        /// Adds a remote peer as a connection to the manager and returns the connection id
        /// </summary>
        public int AddConnection(ENet.ENetPeer* connection)
        {          
            if (count == connections.Length)
            {
                EnsureCapacity(count + 1);
            }
            int connectionId = count;
            connections[connectionId] = connection;

            count++;
            return connectionId;
        }

        public void OnConnect(ENet.ENetPeer* connection)
        {
            if (count == connections.Length)
            {
                EnsureCapacity(count + 1);
            }
                int connectionId = count;
                connections[connectionId] = connection;

                count++;
        }

        /// <summary>
        /// Ensure the internal array not to overflow
        /// </summary>
        private void EnsureCapacity(int min)
        {
            if (connections.Length < min)
            {
                int newCapacity = connections.Length == 0 ? defaultCapacity : connections.Length * 2;

                // 0X7FEFFFFF == Max array length, taken from .NET source
                if ((uint)newCapacity > 0X7FEFFFFF)
                {
                    newCapacity = 0X7FEFFFFF;
                }
                if (newCapacity < min)
                {
                    newCapacity = min;
                    Capacity = newCapacity;
                }
            }
        }

        /// <summary>
        /// Force an additional ping to a remote connection 
        /// </summary>
        public void Ping(int index)
        {
            ENet.PingPeer(connections[index]);
        }

        /// <summary>
        /// Disconnect from all remote connections and cleanup list
        /// </summary>
        public void DisconnectAll()
        {
            for (int i = count; i-- > 0;)
            {
                ENet.DisconnectPeer(connections[i], 1234);
            //    connections[i] = null;
            }
            count = 0;
        }

        /// <summary>
        /// Disconnect a remote connection and remove from list
        /// </summary>
        public void DisconnectConnection(int index)
        {
            ENet.DisconnectPeer(connections[index], 1234);
        //    connections[index] = null;
            count--;
        }

        public void OnDisconnect(ENet.ENetPeer* peer)
        {
            Debug.Log("Received Disconnect Event from: connectID: ", peer->connectID.ToString());
            connections[peer->connectID] = null;
            count--;
        }

        public void Send(int id, OutgoingMessage msg)
        {
            ENet.SendPeer(connections[id], 0 , msg.GetPacket());
        }

        /*
        public ENet.ENetPeer* this[int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)count)
                {
                    Debug.Error("Error in ConnectioManager,  ENet.ENetPeer* property");
                }

                return connections[index];
            }

            set
            {
                if ((uint)index >= (uint)count)
                {
                    Debug.Error("Error in ConnectioManager,  ENet.ENetPeer* property");
                }

                connections[index] = value;
            }
        }
        */

    }
}
