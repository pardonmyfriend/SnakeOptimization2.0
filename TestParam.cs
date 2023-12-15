using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class TestParam
    {
        public double UpperBoundry { get; set; }
        public double LowerBoundry { get; set; }
        public double Step { get; set; }
        public TestParam(double _upperBoundry, double _lowerBoundry, double _step) 
        { 
            UpperBoundry = _upperBoundry;
            LowerBoundry = _lowerBoundry;
            Step = _step;
        }
    }
}
