using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class Results
    {
        public List<int> GenerationsTaken { get; set; }
        public List<String> GenerationResults { get; set; }
        public String Parameters { get; set; }
        public int numOfDecptiveBits { get; set; }

        public Results()
        {
            GenerationsTaken = new List<int>();
            GenerationResults = new List<String>();
            Parameters = "";
            numOfDecptiveBits = 0;
        }
    }
}
