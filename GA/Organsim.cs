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
        public String phenotype { get; set; }
        public int[] genes { get; set; }
        public int numberOfdecptives { get; set; }

        public Organism(int Genes)
        {
            genes = new int[Genes];
        }

        public List<int> getGenes(int from, int to)
        {
            List<int> selGenes = new List<int>();
            for(var i = from  ; i<to; i++)
            {
                selGenes.Add(genes[i]);
            }

            return selGenes;
        }

    }
}
