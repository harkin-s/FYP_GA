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
                    GENERATION[sel].fitness = score;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPerformer = sel;
                    }
                    if(score == GENES)
                    {
                        ideal = true;
                        printOrg(sel);
                    }
                }

                //Calculate Genotype

                tempGen.Add(GENERATION[bestPerformer]);
                //printOrg(bestPerformer);
            }

            // Console.WriteLine("Temp gen has been filled \n " + tempGen.Count);
            GENERATION = generateNextGen(tempGen);
        }

        // Use the temp population to get the next gerneration 
        public static List<Organism> generateNextGen(List<Organism> gen)
        {
            //Add random mutaion latter
           
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
                        nextGen[pop].genes.SetValue(b, nextGen[pop].genes[b] == 1 ? 1 : 0);
                        Console.WriteLine("Mutate");
                    }

                }
            }
            return nextGen;
      
        }

        public static int getPartner(List<Organism> gen ,int sel)
        {
            var testSize = 4;
            var bestPartner = 0;

            
            var score = 0;
            gen[sel].genotype = "";
            var diff = 0;
            for (var a = 0; a < testSize; a++){
                var bestScore = 0;
                var rand = GetRandomNumber(0, 100);
                gen[rand].genotype = "";
                for(int b= 0; b < ALLELES; b++)
                {
                    score = 0;
                    var gene = gen[rand].genes[b].ToString() + gen[rand].genes[b+1].ToString();
                    var tempDiff = 0;
                    
                   // var n = gene.Equals("00");
                    if (gene.Equals("00"))
                    {
                        gen[rand].phenotype = gen[rand].phenotype + "a";
                        gen[sel].phenotype = gen[sel].phenotype + "a";
                    }
                    else if (gene.Equals("01"))
                    {
                        gen[rand].phenotype = gen[rand].phenotype + "b";
                        gen[sel].phenotype = gen[sel].phenotype + "b";
                    }
                    else if (gene.Equals("10"))
                    {
                        gen[rand].phenotype = gen[rand].phenotype + "c";
                        gen[sel].phenotype = gen[sel].phenotype + "c";
                    }
                    else if(gene.Equals("11"))
                    {
                        gen[rand].phenotype = gen[rand].phenotype + "d";
                        gen[sel].phenotype = gen[sel].phenotype + "d";
                    }

                    var l1 = gen[rand].phenotype[b];
                    var l2 = gen[sel].phenotype[b];

                    tempDiff = Math.Abs(l1 - l2);
                    diff = diff + tempDiff;
                    
                }
                score = gen[sel].fitness + diff;
                diff = 0;
                if(score > bestScore)
                {
                    bestScore = score;
                    bestPartner = rand;
                }


            }
            return bestPartner;
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
