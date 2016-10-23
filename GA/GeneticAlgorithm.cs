using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*
    Things to do: Add config file to change global variables such as population ad gene size or global variable

*/
namespace GA
{
    class GeneticAlgorithm
    {
        //Globals
        public static bool ideal = false;
        public static int perfectGen = 0;
        public static int bestGlobalScore = 0;
        public static int ALLELES = 4;
        public static int fitnessWeight = 1;
        public static int phenotypicWeight = 1;
        public static readonly int POPULATION = 100;
        public static readonly int GENES = 8;
        public static List<Organism> GENERATION = new List<Organism>();


        // Main method call the generate population method and all proccedding methods
        static void Main(string[] args)
        {
            //int[,] gen = new int[POPULATION, GENES];
            
            populate();
            getNextGen();
            var count = 0; 
            //while (!ideal)
            //{
            //   getNextGen();
            //   count++;

            //}
           // Console.WriteLine("IT TOOK --->" + count + "<---- GENERATIONS");
            //printOrg(gen, perfectGen)
        }
        //Populates the array with random 1's and 0's
        public static void populate()
        {
            for (int x = 0; x < POPULATION; x++)
            {
                GENERATION.Add(new Organism(GENES));

                for (int y = 0; y < GENES; y++)
                {
                    var ran = GetRandomNumber(1, 10);
                    var num = ran > 5 ? 1 : 0;
                    GENERATION[x].genes.SetValue(num, y);
                }
            }
            
        }
        //Used to get the next generation 
        public static void getNextGen()
        {
            //First evaluate the gen using tournoment selction 
            List<Organism> tempGen = new List<Organism>();
            List<Organism> nextGen = new List<Organism>();
            var compSize = 4;
            int[,] tourn = new int[compSize, GENES];
            //Uses tournment select too fill the next population 

            //Adds random Oragnisms to the test population.
            for (var all = 0; all < POPULATION; all++)
            {
                // tempGen.Add(new Organism(GENES));
                var score = 0;
                var bestScore = 0;
                var bestPerformer = 0;
                // nextGen.Add(new Organism(GENES));
                for (var a = 0; a < compSize; a++)
                {
                    var sel = GetRandomNumber(0, 100);
                    score = 0;
                    for (var c = 0; c < GENES; c++)
                    {
                        score = score + GENERATION[sel].genes[c];

                    }
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPerformer = sel;
                    }
                }
                tempGen.Add(GENERATION[bestPerformer]);
                printOrg(bestPerformer);
            }

            Console.WriteLine("Temp gen has been filled \n " + tempGen.Count);
                            
        }

        // Use the temp population to get the next gerneration 
        public static int[,] mutate(int[,] parents)
        {
            //Add random mutaion latter
           
            var gene = 0;
            int[,] newGen = new int[POPULATION,GENES];
            int[] parentB = new int[GENES];
            //Cross over first 
            
            for (var pop = 0; pop < POPULATION - 1; pop++)
            {
                //parentB = getPartner(parents, pop);
                var crossPoint = GetRandomNumber(0, 20);
            
                for (var b = 0; b < GENES; b++)
                {
                    var random = GetRandomNumber(0, 1000);
                     if (b <= crossPoint)
                    {
                        gene = parents[pop, b];
                        newGen[pop, b] = gene;
                       
                    }
                    else 
                    {
                        gene = parents[pop+1 ,b];
                        newGen[pop, b] = gene;
                    }

                    //if (random == 1)
                    //{
                    //    gene = parent[pop, b] == 1 ? 0 : 1;
                    //    newGen[pop, b] = gene;
                    //    Console.WriteLine("MUTAION");

                    //}

                }
            }
            return newGen;
      
        }

        public static int getPartner(int[,] gen, int sel)
        {
            var testSize = 4;
            var bestPartner = 0;
            
            for (var a = 0; a < testSize; a++){
            var rand = GetRandomNumber(0, 100);
                for(int b= 0; b < 4; b++)
                {
                    var current = 0;

                }


            }
            return 0;
        }

        //Utiltiy Functions 


        public static void copyPopulation(int [,] from)
        {
            for(var b = 0; b<POPULATION; b++)
            {
                for(var a = 0; a<GENES; a++)
                {
                    var gene = from[b ,a];
                  //  GEN[b, a] = gene;           
                }
            }

        }

        public static void printOrg(int num)
        {
            Console.WriteLine("ORGANISM ---->" + num + "<----");
            for (int p = 0; p < GENES; p++)
            {

                Console.WriteLine(GENERATION[num].genes[p]);

            }
        }
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return getrandom.Next(min, max);
            }
        }

    }
}
