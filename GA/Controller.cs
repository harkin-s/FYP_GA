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
            var runTime = 100;
            int[] ans = new int[runSize];
            String[] results = new String[runTime];
            
           
            for (var a = 0; a < runTime; a++)
            {
                ans = GeneticAlgorithm.runGA(runSize);
              
                if (ans.Sum() == 0)
                {
                    Console.WriteLine("No ideal has been found");
                }
                Console.WriteLine("The Average of the Results are: " + ans.Average());
                results[a] = ans.Average().ToString();
            }
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Sean\Documents\FYP_Results\results.txt"))
            {
                foreach (String res in results)
                {             
                    file.WriteLine(res);   
                }
            }
        }
    }
}
