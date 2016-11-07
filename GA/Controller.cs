using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class Controller
    {
        public static void Main(String[] args)
        {
            var runSize = 100;
            var total = 0;
            int[] ans = new int[runSize];
            ans = GeneticAlgorithm.runGA(runSize);

            for(var it = 0; it < runSize; it++)
            {
               
                Console.WriteLine("Result 1:" + ans[it]);
                total = total + ans[it];
                
            }
            Console.WriteLine("The Average of the Results are: " + total / runSize);
        }
    }
}
