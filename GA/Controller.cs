using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class Controller
    {
        public static String experiment = "Experimetn.5.UniformCross";
        public static String runtype = "Uniform_8DEC_Pheno";
        public static void Main(String[] args)
        {

            var runSize = 100;
            var runTime = 100;
            List<float> ans = new List<float>();
            List<String> results = new List<String>();
            List<float> allResults = new List<float>();
            List<float> numofDecptive = new List<float>();
            List<String> allGenerationBests = new List<String>();
            List<float> deceptivesInRun = new List<float>();
            results.Add("Average,Best,Worst");
            var par = "";
            for (var a = 0; a < runTime; a++)
            {
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
                ans.Clear();
            }
            results.Add(par);
            outputRunResults(allResults);
            outputGenerationBestResults(allGenerationBests);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("C:\\Users\\shark\\Documents\\FYP_Results\\" + experiment + "\\results_average_" + runtype +"_"+  DateTime.Now.ToShortDateString()+".csv"))
            {
                foreach (String res in results)
                {             
                    file.WriteLine(res);
                }
            }

            outputSummation(results, allResults, allGenerationBests);
        }

        private static void outputRunResults(List<float> ans)
        {
            
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("C:\\Users\\shark\\Documents\\FYP_Results\\" + experiment + "\\results_all_" + runtype + "_"+ DateTime.Now.ToShortDateString() + ".csv"))
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
            new System.IO.StreamWriter("C:\\Users\\shark\\Documents\\FYP_Results\\" + experiment + "\\generationBest_" + runtype + "_" + DateTime.Now.ToShortDateString() + ".csv"))
            {

                foreach (String a in ans)
                {
                    file.WriteLine(a);
                }
            }
        }

        private static void outputSummation(List<String> avg , List<float> allRuns, List<String> genBest)
        {

            List<String> ans = new List<String>();

            var fails = from f in allRuns
                        where f >= 100
                        select f;

            float failRate = (float)( fails.Count() / (float)allRuns.Count()) * 100;

            ans.Add("Fail Rate" + "," + failRate.ToString());

            ans.Add("Average All " + "," + allRuns.Average());

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("C:\\Users\\shark\\Documents\\FYP_Results\\"+experiment+ "\\summary_" + runtype + "_"+ DateTime.Now.ToShortDateString() + ".csv"))
            {

                foreach (String a in ans)
                {
                    file.WriteLine(a);
                }
            }
        }
    }
}
