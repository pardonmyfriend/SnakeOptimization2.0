﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public interface IGeneratePDFReport
    {
        void GenerateReport(string path, string reportString);
    }
}
