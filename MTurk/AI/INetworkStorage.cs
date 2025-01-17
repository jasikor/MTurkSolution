﻿using NeuralNetworkNET.APIs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.AI
{
    public interface INetworkStorage
    {
        INeuralNetwork Load();
        void Save(INeuralNetwork network);
    }
}
