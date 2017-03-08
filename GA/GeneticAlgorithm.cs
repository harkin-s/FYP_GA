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

    01/03/2017  - Should get similar results as before may be hamming distance causing error review all of the code.
                  Store data form all runs not just average and plot to show convergence.
                  Also add elitiesm to select the best candiated from each genetartion to go through without competiton
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
        public static int generationBestPerformer = 0;
        public static List<int> alleleSizes = new List<int>();    
        public static int fitnessWeight = 1;
        public static int phenotypicWeight = 1;
        private static int totalDecptiveBits = 0;
        public static readonly int POPULATION = 100;
        public static readonly int GENES = 30;
        public static int ALLELES = 0;      //set to 8 as need 
        public static int deceptiveReward = 30;
        public static bool deceptiveLandscape = false;
        public static bool usePehnotype = false;
        private static bool multipleDeceptives = false;
        private static bool varyAlleles = false;
        private static bool weightedCrossover = false;
        private static bool elitism = false;
        private static int numWithDecptive = 0;
        private static int numberOfDecptiveLandscape = 0;   // If set to zero will choose random amount of decptives will only work if multipleDeceptives is true otherwise will just add one a random position 
        public static List<Organism> GENERATION = new List<Organism>();
        


        // Main method call the generate population method and all proccedding methods
        public static Results runGA(int iterations)
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
                if (multipleDeceptives)
                {
                    var numOfDec = GetRandomNumber(1, ALLELES);
                    numberOfDecptiveLandscape = numberOfDecptiveLandscape > 0 ? numberOfDecptiveLandscape: numOfDec;
                    while (deceptiveLocations.Count != numberOfDecptiveLandscape)
                    {
                        var loc = GetRandomNumber(1, ALLELES);
                        if (!deceptiveLocations.Contains(loc))
                        {
                            deceptiveLocations.Add(loc);
                        }
                    }
                    // gnumOfDec = numberOfDecptiveLandscape;

                    deceptiveLocations.Sort();
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
                   totalDecptiveBits += alleleSizes[n];   
            }

            Results results = new Results();
            for (var runs = 0; runs < iterations; ++runs)
            {
                populate();
                var count = 0;
                
                while (!ideal)
                {
                   // Console.WriteLine("Num of Org with deceptive: " + numWithDecptive + " At gen: " +count);
                    numWithDecptive = 0;
                    getNextGen();
                    count++;
                    results.GenerationBestPerformer.Add(generationBestPerformer.ToString() + "," + count);
                    generationBestPerformer = 0;
                    //Console.WriteLine("Best Score --> "+bestGlobalScore);
                    if (count >= 100)
                    {
                        ideal = true;
                        //Console.WriteLine("No optimal has been found after 100 genertaions");
                    }
                    //Console.WriteLine("AT --->" +count + "<---- GENERATIONS" + "Best Score is -->" + bestGlobalScore);
                }
                //Console.WriteLine("IT TOOK --->" + count + "<---- GENERATIONS");
                ideal = false;
                results.GenerationsTaken.Add(count);
            }
            results.Parameters = "Number Of Cross Points:" + numOfCrossPoints + ",Fitness weight:" + fitnessWeight + ",Phenotypic weight: " + phenotypicWeight +
                ",Population Size: " + POPULATION + ",Number of Genes: " + GENES + ",Deceptive Lanscape" + deceptiveLandscape + ",Phenotypic matching: " + usePehnotype +
                ",Vary decptive position: " + multipleDeceptives + ",Vary Allele sizes: " + varyAlleles + ",Weighted crossover: " + weightedCrossover + ",Elitism: " + elitism +
                ",Number of deceptive lanscapes: " + numberOfDecptiveLandscape;
            results.numOfDecptiveBits = totalDecptiveBits;
            return results;
            
        }

        //Populates the array with random 1's and 0's
        public static void populate()
        {
            GENERATION.Clear();
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
                    var sel = GetRandomNumber(0, POPULATION);
                    score = 0;

                    if (deceptiveLandscape)
                        checkDecptives(GENERATION[sel]);

                    for (var c = 0; c < GENES; c++)
                    {
                        score = score + GENERATION[sel].genes[c];
                    }
                    GENERATION[sel].fitness = score;
                    
                    score += (GENERATION[sel].numberOfdecptives * deceptiveReward);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPerformer = sel;
                    }
                    
                }
                generationBestPerformer = generationBestPerformer < bestScore ? bestScore : generationBestPerformer;
                tempGen.Add(GENERATION[bestPerformer]);
                
            }

            checkForIdeal(GENERATION);
            GENERATION = generateNextGen(tempGen);
        }

        // Use the temp population to get the next gerneration using crossover and partner selection
        public static List<Organism> generateNextGen(List<Organism> gen)
        {
            List<int> gene = new List<int>();
            List<int> crossPoitns = new List<int>();
            List<Organism> nextGen = new List<Organism>();
            if (elitism)
            {
                Organism elite = new Organism(GENES);
                foreach (Organism org in gen)
                {
                    if(org.fitness+org.numberOfdecptives > elite.fitness + elite.numberOfdecptives)
                    {
                        elite = org;
                    }
                }
                nextGen.Add(elite);
            }

            //Cross over first 

            
            for (var pop = nextGen.Count ; pop < POPULATION; pop++)
            {
                // Get best partner for organism 
                var parentB = getPartner(gen, pop);
                nextGen.Add(new Organism(GENES));

                if (!weightedCrossover)
                {
                    while (crossPoitns.Count != numOfCrossPoints)
                    {
                        var point = GetRandomNumber(0, GENES);
                        if (!crossPoitns.Contains(point))
                        {
                            crossPoitns.Add(point);
                        }
                    }
                }
                else if (weightedCrossover && numOfCrossPoints == 1)
                {
                    float fitnessA = gen[pop].fitness + (gen[pop].numberOfdecptives * 10);
                    float fitnessB = gen[parentB].fitness + (gen[parentB].numberOfdecptives * 10);
                    float com = fitnessA + fitnessB;
                    float cross = (fitnessA / com);
                    //var corssB = fitnessB / fitnessA + fitnessB;
                    cross = cross * GENES;
                    crossPoitns.Add((int)cross);
                }


                crossPoitns.Add(0);
                crossPoitns.Sort();

                // This is the cross over algorithm uses random cross point.
                // Test use of different number of cross points 
                // Maybe add weighted cross over or min differenc between cross points 
                // Add ability for multiple cross over.
                // Should cross points change for each cross over or stay the same for each generation 

                var endPoint = 0;
                var parent = 0;
                for (var b = 0; b < crossPoitns.Count; b++)
                {
                    endPoint = b+1 < crossPoitns.Count  ? crossPoitns[b + 1] : GENES;  
                    if (parent == 0)     // Number is even
                    {
                        gene = gen[pop].spliceGenes(crossPoitns[b], endPoint);
                        nextGen[pop].addGenes(gene, crossPoitns[b], endPoint);
                        parent = 1;
                      
                    }
                    else if (parent == 1)
                    {
                        gene = gen[parentB].spliceGenes(crossPoitns[b], endPoint);
                        nextGen[pop].addGenes(gene, crossPoitns[b], endPoint);
                        parent = 0; 
                    }

                    nextGen[pop] = mutate(nextGen[pop]);
                }
                crossPoitns.Clear();
                //Console.WriteLine("At Population: "+ pop);
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
                score = ((fitnessWeight * orgB.fitness) + (orgB.numberOfdecptives * deceptiveReward )) + (phenotypicWeight * diff);
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
        public static Organism mutate(Organism org)
        {
            for(var g = 0; g < org.genes.Length; g++) {

                var random = GetRandomNumber(0, 100);
                //Mutaion of gene
                if (random == 1)
                {

                    if (org.genes[g] == 0)
                    {
                        org.genes[g] = 1;
                    }
                    else
                    {
                        org.genes[g] = 0;
                    }
                }
            }
            return org;
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

                        //printOrg(org);
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

                var val = ListToString(gene);
                if (deceptiveLocations.Contains(x) && val.Contains("000") && alleleSizes[x] == 3)
                {
                    org.numberOfdecptives++;
                    numWithDecptive++;
                     
                }
                
                else if (deceptiveLocations.Contains(x) && alleleSizes[x] ==5 && val.Contains("00000"))         //Set to 2 for now as deceptive landscape is a three zeros need to change later!
                {
                    org.numberOfdecptives++;
                    numWithDecptive++;
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

        public static String ListToString(List<int> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int i in list)
            {
                builder.Append(i);
            }
            return builder.ToString();
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
