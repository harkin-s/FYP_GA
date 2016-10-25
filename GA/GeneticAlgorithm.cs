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
        public static int ALLELES = 12;
        public static int fitnessWeight = 1;
        public static int phenotypicWeight = 1;
        public static readonly int POPULATION = 100;
        public static readonly int GENES = 24;
        public static List<Organism> GENERATION = new List<Organism>();


        // Main method call the generate population method and all proccedding methods
        static void Main(string[] args)
        {
            populate();
            var count = 0;
            while (!ideal)
            {
                getNextGen();
                count++;

            }
            Console.WriteLine("IT TOOK --->" + count + "<---- GENERATIONS");
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
            var compSize = 4;
            int[,] tourn = new int[compSize, GENES];
            var bestPerformer = 0;
            //Uses tournment select too fill the next population 

            //Adds random Oragnisms to the test population.
            for (var all = 0; all < POPULATION; all++)
            {
                // tempGen.Add(new Organism(GENES));
                var score = 0;
                var bestScore = 0;
                bestPerformer = 0;
                // nextGen.Add(new Organism(GENES));
                for (var a = 0; a < compSize; a++)
                {
                    var sel = GetRandomNumber(0, 100);
                    score = 0;
                    for (var c = 0; c < GENES; c++)
                    {
                        score = score + GENERATION[sel].genes[c];
                    }
                    GENERATION[sel].fitness = score;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPerformer = sel;
                    }
                    if (score == GENES)
                    {
                        ideal = true;
                        printOrg(sel);
                    }
                }

                var x = 0;
                var y = 1;
                GENERATION[bestPerformer].phenotype = "";
                for (int b = 0; b < ALLELES; b++)
                {

                    if (b > 0)
                    {
                        x = x + 2;
                        y = y + 2;
                    }
                    var gene = GENERATION[bestPerformer].genes[x].ToString() + GENERATION[bestPerformer].genes[y].ToString();

                    if (gene.Equals("00"))
                    {
                        GENERATION[bestPerformer].phenotype = GENERATION[bestPerformer].phenotype + "a";
                    }
                    else if (gene.Equals("01"))
                    {
                        GENERATION[bestPerformer].phenotype = GENERATION[bestPerformer].phenotype + "b";
                    }
                    else if (gene.Equals("10"))
                    {
                        GENERATION[bestPerformer].phenotype = GENERATION[bestPerformer].phenotype + "c";
                    }
                    else if (gene.Equals("11"))
                    {
                        GENERATION[bestPerformer].phenotype = GENERATION[bestPerformer].phenotype + "d";
                    }
                    else
                    {
                        throw new System.ArgumentException("ERROR!! -- Gene pair not found");
                    }
                }
                tempGen.Add(GENERATION[bestPerformer]);
            }
            GENERATION = generateNextGen(tempGen);
        }

        // Use the temp population to get the next gerneration 
        public static List<Organism> generateNextGen(List<Organism> gen)
        {
            var gene = 0;
            //Cross over first 
            List<Organism> nextGen = new List<Organism>();
            for (var pop = 0; pop < POPULATION; pop++)
            {
                var parentB = getPartner(gen, pop);
                nextGen.Add(new Organism(GENES));
                var crossPoint = GetRandomNumber(0, 20);

                for (var b = 0; b < GENES; b++)
                {
                    var random = GetRandomNumber(0, 1000);
                    if (b <= crossPoint)
                    {
                        gene = gen[pop].genes[b];
                        nextGen[pop].genes[b] = gene;

                    }
                    else
                    {
                        gene = gen[parentB].genes[b];
                        nextGen[pop].genes[b] = gene;
                    }

                    if (random == 1)
                    {
                        if(nextGen[pop].genes[b] == 0)
                        {
                            nextGen[pop].genes[b] = 1;
                        }
                        else
                        {
                            nextGen[pop].genes[b] = 0;
                        }
                        Console.WriteLine("Mutate");
                    }

                }
            }
            return nextGen;

        }

        public static int getPartner(List<Organism> gen, int sel)
        {
            var testSize = 4;
            var bestPartner = 0;
            var score = 0;
            var diff = 0;
            var bestScore = 0;
            for (var a = 0; a < testSize; a++)
            {
                var rand = GetRandomNumber(0, 100);
                for (var b = 0; b < ALLELES; b++)
                {
                    var l1 = gen[rand].phenotype[b];
                    var l2 = gen[sel].phenotype[b];
                    var tempDiff = 0;
                    tempDiff = Math.Abs(l1 - l2);
                    diff = diff + tempDiff;
                }
                score = gen[sel].fitness + diff;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPartner = rand;
                }
            }

            return bestPartner;
        }

        //Utiltiy Functions 


        public static void copyPopulation(int[,] from)
        {
            for (var b = 0; b < POPULATION; b++)
            {
                for (var a = 0; a < GENES; a++)
                {
                    var gene = from[b, a];
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
