using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public class DiskNetworkStorage : INetworkStorage
    {
        private readonly FileInfo _file;
        public DiskNetworkStorage()
        {
            _file = new FileInfo("test.nnet");
        }
        public INeuralNetwork Load()
        {
            return  NetworkLoader.TryLoad(_file, ExecutionModePreference.Cpu);
        }

        public void Save(INeuralNetwork network)
        {
            network.Save(_file);
        }
    }
}
