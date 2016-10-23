using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    //This class will define an organism to be used in the populations.
    public class Organism
    {
        public int fitness { get; set; }
        public int genotype { get; set; }
        public int[] genes { get; set; }

        public Organism(int Genes)
        {
            genes = new int[Genes];
        }

    }
}
