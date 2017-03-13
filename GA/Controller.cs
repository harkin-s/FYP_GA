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
            List<int> ans = new List<int>();
            List<String> results = new List<String>();
            List<int> allResults = new List<int>();
            List<int> numofDecptive = new List<int>();
            List<String> allGenerationBests = new List<String>();
            List<int> deceptivesInRun = new List<int>();
            results.Add("Average,Best,Worst");
            var par = "";
            for (var a = 0; a < runTime; a++)
            {
                var res = new Tuple<List<int>, string>(ans, "");
                Results resGA = GeneticAlgorithm.runGA(runSize);
                ans = resGA.GenerationsTaken;
                par = resGA.Parameters;
                if (a < 1)
                {
                    allGenerationBests.AddRange(resGA.GenerationResults);
                    deceptivesInRun.Add(resGA.numOfDecptiveBits);

                }

                allResults.AddRange(ans);
                if (ans.Sum() == 0)
                {
                    Console.WriteLine("No ideal has been found");
                }
                Console.WriteLine("The Average of the Results are: " + ans.Average() + " At run: " + a);
                results.Add(ans.Average() + "," + ans.Min() + "," + ans.Max());
            }
            results.Add(par);
            outputRunResults(allResults);
            outputGenerationBestResults(allGenerationBests);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\shark\Documents\FYP_Results\results_average_"+DateTime.Now.ToShortDateString()+".csv"))
            {
                foreach (String res in results)
                {             
                    file.WriteLine(res);
                }
            }

        
        }

        private static void outputRunResults(List<int> ans)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\shark\Documents\FYP_Results\results_all_" + DateTime.Now.ToShortDateString() + ".csv"))
            {

                foreach (int a in ans)
                {
                    file.WriteLine(a);
                }
            }
        }
        private static void outputGenerationBestResults(List<String> ans)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\shark\Documents\FYP_Results\generationBest_" + DateTime.Now.ToShortDateString() + ".csv"))
            {

                foreach (String a in ans)
                {
                    file.WriteLine(a);
                }
            }
        }
    }
}
