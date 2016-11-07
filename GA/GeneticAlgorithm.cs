using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*
   TODO: Check phenotype is working properley 
         add more genes 30 gense to make 10 sub problems 
         intergrate gnuplot to make charts of the results 
         add ability to decide to run with phenotype or not 
         add config 
         run multiplie times to gain accurate results
         make output to text file 
*/
namespace GA
{
    class GeneticAlgorithm
    {
        //Globals
        public static bool ideal = false;
        public static int perfectGen = 0;
        public static int uniqueLocation = 5;
        public static int bestGlobalScore = 0;
        public static int groupSize = 3;    //Must evenley divide 
        public static int fitnessWeight = 1;
        public static int phenotypicWeight = 1;
        public static readonly int POPULATION = 100;
        public static readonly int GENES = 30;
        public static int ALLELES = GENES/groupSize;
        public static int deceptiveReward = 50;
        public static bool usePehnotype = false;
        public static List<Organism> GENERATION = new List<Organism>();


        // Main method call the generate population method and all proccedding methods
        public static int[] runGA(int iterations)
        {
            int[] results = new int[iterations];
            for (var runs = 0; runs < iterations; ++runs)
            {
                populate();
                var count = 0;
                while (!ideal)
                {
                    getNextGen();
                    count++;
                    // Console.WriteLine("AT --->" +count + "<---- GENERATIONS" + "Best Score is -->" + bestGlobalScore);
                }
               // Console.WriteLine("IT TOOK --->" + count + "<---- GENERATIONS");
                results[runs] = count;
                ideal = false;
                //printOrg(gen, perfectGen)
                Console.WriteLine("At Run: " + runs);
            }
            return results;
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
            tempGen.Clear();
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
                        if (score >= GENES && !usePehnotype)
                        {
                            ideal = true;
                           // printOrg(bestPerformer);
                        }

                    }
                    bestGlobalScore = bestGlobalScore < bestScore ? bestScore : bestGlobalScore;
                }            
                //printOrg(bestPerformer);
                tempGen.Add(usePehnotype ? getPehnotype(bestPerformer, bestScore): GENERATION[bestPerformer]);
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
                var crossPoint = GetRandomNumber(0, GENES);

                for (var b = 0; b < GENES; b++)
                {
                    var random = GetRandomNumber(0, 4000);
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
                        if (nextGen[pop].genes[b] == 0)
                        {
                            nextGen[pop].genes[b] = 1;
                        }
                        else
                        {
                            nextGen[pop].genes[b] = 0;
                        }
                        //Console.WriteLine("Mutate");
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
                var rand = GetRandomNumber(0, POPULATION);
                for (var b = 0; b < ALLELES; b++)
                {
                    if (usePehnotype)
                    {
                        var l1 = gen[rand].phenotype[b];
                        var l2 = gen[sel].phenotype[b];
                        var tempDiff = 0;
                        tempDiff = Math.Abs(l1 - l2);
                        diff = diff + tempDiff;
                    }
                }
                score = (fitnessWeight * gen[sel].fitness) + (phenotypicWeight * diff);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPartner = rand;
                }
            }

            return bestPartner;
        }
        public static Organism getPehnotype(int organism ,int score)
        {
            var x = 0;
            var y = 1;
            var z = 2;
            GENERATION[organism].phenotype = "";
            GENERATION[organism].hasIdealGene = false;
            for (int b = 0; b < ALLELES; b++)
            {
                var location = b == (ALLELES - 1) ? true : false;
                if (b > 0)
                {
                    x = x + groupSize;
                    y = y + groupSize;
                    z = z + groupSize;
                }
                var gene = GENERATION[organism].genes[x].ToString() + GENERATION[organism].genes[y].ToString() + GENERATION[organism].genes[z].ToString();

                switch (gene)
                {
                    case "001":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "a";
                        break;
                    case "010":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "b";
                        break;
                    case "100":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "c";
                        break;
                    case "011":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "d";
                        break;
                    case "110":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "e";
                        break;
                    case "101":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "f";
                        break;
                    case "111":
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "g";
                        break;
                    case "000": //Rewarding solution for finding the deceptive peak.

                        if (location)
                        {
                            //GENERATION[bestPerformer].phenotype = GENERATION[bestPerformer].phenotype + "x";
                            GENERATION[organism].hasIdealGene = true;
                            GENERATION[organism].fitness = GENERATION[organism].fitness + deceptiveReward;
                            Console.WriteLine("HAS FOUND DECEPTIVE LANDSCAPE" + "---->" + GENERATION[organism].fitness);
                            printOrg(organism);
                        }
                        GENERATION[organism].phenotype = GENERATION[organism].phenotype + "h";

                        break;
                    default:
                        Console.WriteLine("Type not found");
                        break;
                }

            }
            if (score >= (GENES - 3) && GENERATION[organism].hasIdealGene)
            {
                ideal = true;
                printOrg(organism);
            }

            return GENERATION[organism];

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
