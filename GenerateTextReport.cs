using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeOptimization
{
    public class GenerateTextReport : IGenerateTextReport
    {
        public string ReportString { get; set; }

        public GenerateTextReport(string report)
        {
            ReportString = report;
        }
    }
}
