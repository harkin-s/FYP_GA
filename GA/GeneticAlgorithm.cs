using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<int> deceptiveLocations = new List<int>();
        private static int numOfCrossPoints = 1;        //Determines how many cross points the cross over algoithm will have
        public static int bestGlobalScore = 0;
        public static List<int> alleleSizes = new List<int>();    //Must evenley divide 
        public static int fitnessWeight = 3;
        public static int phenotypicWeight = 1;
        private static int totalDecptiveBits = 0;
        public static readonly int POPULATION = 100;
        public static readonly int GENES = 30;
        public static int ALLELES = 0;      //set to 8 as need 
        public static int deceptiveReward = 300;
        private static int gnumOfDec = 0;
        public static bool deceptiveLandscape = true;
        public static bool usePehnotype = true;
        private static bool varyDecptivePosition = false;
        private static bool varyAlleles = false;
        private static int numWithDecptive = 0;
        public static List<Organism> GENERATION = new List<Organism>();


        // Main method call the generate population method and all proccedding methods
        public static int[] runGA(int iterations)
        {

            //Set up for varying allelies can only contain 3 or 5 .can only contain 3 x 5 alleles and 5 x 3 .Num of alleles will be eight
            if (varyAlleles)
            {
                alleleSizes.Clear();

                List<int> posOf5s = new List<int>();
                posOf5s.Clear();

                while (posOf5s.Count != 3)
                {
                    var posOf5 = GetRandomNumber(0, 7);
                    if (!posOf5s.Contains(posOf5))
                    {
                        posOf5s.Add(posOf5);
                    }
                }
                posOf5s.Sort();
                var listPos = 0;
                for (var i = 0; i < 8; i++)
                {

                    if (i == posOf5s[listPos] && i <= posOf5s[2])
                    {
                        alleleSizes.Add(5);
                        if (listPos < 2)
                        {
                            listPos++;
                        }
                    }
                    else
                    {
                        alleleSizes.Add(3);
                    }
                }
                ALLELES = alleleSizes.Count;
            }
            else
            {
                ALLELES = GENES / 3;
                for(var b=0; b< ALLELES; b++)
                {
                    alleleSizes.Add(3);
                }
            }


            //Deceptive locations set at random 
            deceptiveLocations.Clear();
            if (deceptiveLandscape)
            {
                if (varyDecptivePosition)
                {
                    var numOfDec = GetRandomNumber(1, ALLELES);
                    while (deceptiveLocations.Count != numOfDec)
                    {
                        var loc = GetRandomNumber(1, ALLELES);
                        if (!deceptiveLocations.Contains(loc))
                        {
                            deceptiveLocations.Add(loc);
                        }
                    }
                    gnumOfDec = numOfDec;
                }
                else
                {
                    var rand = GetRandomNumber(1, ALLELES);
                    deceptiveLocations.Add(rand);
                }

            }

            totalDecptiveBits = 0;
            //Finds the total sum of decptive bits
            foreach (int n in deceptiveLocations)
            {
                for (var a = 0; a < alleleSizes.Count; a++)
                {
                    if (n == a)
                        totalDecptiveBits += alleleSizes[a];
                }
            }


            int[] results = new int[iterations];
            for (var runs = 0; runs < iterations; ++runs)
            {
                populate();
                var count = 0;
                while (!ideal)
                {
                    Console.WriteLine("Num of Org with deceptive: " + numWithDecptive + " At gen: " +count);
                    numWithDecptive = 0;
                    getNextGen();
                    count++;
                    if(count >= 100)
                    {
                        ideal = true;
                        //Console.WriteLine("No optimal has been found after 100 genertaions");
                    }
                    // Console.WriteLine("AT --->" +count + "<---- GENERATIONS" + "Best Score is -->" + bestGlobalScore);
                }
               // Console.WriteLine("IT TOOK --->" + count + "<---- GENERATIONS");
                results[runs] = count;
                ideal = false;
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
            var bestPerformer = 0;
            //Uses tournment select too fill the next population 

            //Adds random Oragnisms to the test population.
            for (var all = 0; all < POPULATION; all++)
            {
                // tempGen.Add(new Organism(GENES));
                var score = 0;
                var bestScore = 0;
                bestPerformer = 0;

                // This will select four at random and choose the best to into next generation. 
                for (var a = 0; a < compSize; a++)
                {
                    var sel = GetRandomNumber(0, 100);
                    score = 0;
                    for (var c = 0; c < GENES; c++)
                    {
                        score = score + GENERATION[sel].genes[c];
                    }
                    GENERATION[sel].fitness = score;
                    if(deceptiveLandscape)
                        checkDecptives(GENERATION[sel]);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPerformer = sel;
                    }
                    bestGlobalScore = bestGlobalScore < bestScore ? bestScore : bestGlobalScore;
                }
                tempGen.Add(GENERATION[bestPerformer]);
                
            }
            GENERATION = generateNextGen(tempGen);
        }

        // Use the temp population to get the next gerneration using crossover and partner selection
        public static List<Organism> generateNextGen(List<Organism> gen)
        {
            List<int> gene = new List<int>();
            List<int> tempGenes = new List<int>();
            //Cross over first 
            List<Organism> nextGen = new List<Organism>();
            for (var pop = 0; pop < POPULATION; pop++)
            {
                // Get best partner for organism 
                var parentB = getPartner(gen, pop);
                nextGen.Add(new Organism(GENES));

                //This is the cross over algorithm uses random cross point
                // Maybe add weighted cross over.
                //Add ability for multiple cross over.
                List<int> crossPoitns = new List<int>();

                while(crossPoitns.Count != numOfCrossPoints)
                {
                    var point = GetRandomNumber(0, GENES);
                    if (!crossPoitns.Contains(point))
                    {
                        crossPoitns.Add(point);
                    }
                }
                crossPoitns.Sort();
                var startPoint = 0;
                var endPoint = 0;
                for (var b = 0; b <= crossPoitns.Count; b++)
                {
                    endPoint = b == crossPoitns.Count-1 ? crossPoitns[b] : GENES;
                    var random = GetRandomNumber(0, 4000);  //Random mutation factor 0.025% chance 
                    if (b % 2 == 0)     // Number is even
                    {
                        gene = gen[pop].spliceGenes(startPoint, endPoint);
                        nextGen[pop].addGenes(gene, startPoint,endPoint);
                        startPoint += crossPoitns[b];
                      
                    }
                    else if (b % 2 != 0)
                    {
                        gene = gen[parentB].spliceGenes(startPoint, endPoint);
                        nextGen[pop].addGenes(gene, startPoint, endPoint);
                        startPoint += crossPoitns[b];
                    }
                    //Mutaion of gene
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
                       // Console.WriteLine("Mutate");
                    }

                }
            }

            checkForIdeal(nextGen);
            return nextGen;

        }

        public static int getPartner(List<Organism> gen, int sel)
        {
            var testSize = 4;
            var bestPartner = 0;
            
            var bestScore = 0;
            for (var a = 0; a <= testSize; a++)
            {
                var diff = 0;
                var score = 0;
                var rand = GetRandomNumber(0, POPULATION);
                Organism orgA = gen[sel];
                Organism orgB = gen[rand];
                if(usePehnotype)
                diff = getHammingDistance(orgA, orgB);
                // Gets the score for each partner will select one with the highest score for crossover.
                // Score is a sum of the fitness of the organism plus any deceptives the orgainims has and its phenotypic difference from the the select organism
                score = ((fitnessWeight * gen[sel].fitness) + (gen[sel].numberOfdecptives * deceptiveReward )) + (phenotypicWeight * diff);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPartner = rand;
                }
            }

            return bestPartner;
        }

        public static int getHammingDistance(Organism orgA, Organism orgB)
        {
            var score = 0;
            for (var a = 0; a < GENES; a++)
            {
                if(orgA.genes[a] != orgB.genes[a])
                {
                    score++;
                }

            }
            return score;
        }

        public static void checkForIdeal(List<Organism> gen)
        {
            foreach(Organism org in gen)
            {
                var fitness = from g in org.genes
                              where g > 0
                              select g;

                org.fitness = fitness.Sum();
                if (deceptiveLandscape)
                {
                    /*
                    var ty = org.getGenes(deceptiveLocations[0]*3, deceptiveLocations[0]*3 + 3);
                    foreach(int i in ty)
                    {
                        Console.WriteLine(i);
                    }
                    */
                        
                    if (org.numberOfdecptives == deceptiveLocations.Count && org.fitness == (GENES - totalDecptiveBits))
                    {
                        ideal = true;
                        printOrg(org);
                    }
                }
                else if (!deceptiveLandscape && org.fitness >= GENES)
                {
                    ideal = true;
                }
            }
        }

        public static void checkDecptives(Organism org)
        {
            org.numberOfdecptives = 0;
            var currStartCut = 0;
            var endCut = 0;
            
            for(var x = 0; x<ALLELES; x++)
            {
                List<int> gene = new List<int>();
                endCut = x > 0 ? alleleSizes[x] + endCut : alleleSizes[x];
                gene = org.spliceGenes(currStartCut, endCut);
                currStartCut += alleleSizes[x];

                var val = from a in gene
                          where a > 0
                          select a;
                
                if (deceptiveLocations.Contains(x) && val.Sum() == 0 )
                {
                    org.numberOfdecptives++;
                    numWithDecptive++;
                    //Console.WriteLine(" Decptive found !!");
                    //printOrg(org);
                }
            }
        }

        //Add check if deceptive landscapes in Organism
     


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

        public static void printOrg(Organism org)
        {
            Console.WriteLine("ORGANISM Genes");
            for (int p = 0; p < GENES; p++)
            {

                Console.WriteLine(org.genes[p]);

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
