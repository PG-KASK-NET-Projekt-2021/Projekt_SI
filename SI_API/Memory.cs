using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SI_API;

namespace SI_API
{
    public class Memory
    {
        private readonly int size = 20;
        public readonly List<SensorData> Mem;

        public Memory()
        {
            Mem = new List<SensorData>();
        }

        public void Add(SensorData data)
        {
            Mem.Add(data);
            if (Mem.Count > size)
                Mem.RemoveAt(0);
        }
    }
    
}