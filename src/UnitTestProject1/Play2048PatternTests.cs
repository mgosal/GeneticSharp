using ExtensionLibrary;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class Play2048PatternTests
    {
        int bestGen = 0;
        IChromosome bestChromosome;
        [TestMethod]
        public void Play2048PatternTest()
        {
            
            var selection = new EliteSelection();
            var crossover = new CutAndSpliceCrossover();
            var mutation = new PartialShuffleMutation();
            var fitness = new ClosestToMillion();
            var chromosome = new Play2048Chromosome(20, new List<string>() { "up", "down", "left", "right" });
            var population = new CustomPopulation(2, 4, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(10);

            Console.WriteLine("GA running...");
            ga.GenerationRan += Ga_GenerationRan;
            
            
            Console.WriteLine("Best solution found has {0} fitness was born in generation {1}", ga.BestChromosome.Fitness, bestGen);
        }

        private void Ga_GenerationRan(object sender, EventArgs e)
        {
            var ga = sender as GeneticAlgorithm;
            if (bestChromosome == null)
            {
                bestChromosome = ga.BestChromosome;
                bestGen = ga.GenerationsNumber;
            }
            if (ga.BestChromosome.Fitness > bestChromosome.Fitness)
            {
                bestChromosome = ga.BestChromosome;
                bestGen = ga.GenerationsNumber;
            }
            Console.WriteLine("Generation {0}, {1}", ga.GenerationsNumber, (ga.BestChromosome as Play2048Chromosome).PatternLength);
        }

        public class ClosestToMillion : IFitness
        {
            public double Evaluate(IChromosome chromosome)
            {
                // Evaluate the fitness of chromosome.
                
                var play2048Chromosome = chromosome as Play2048Chromosome;
                if (play2048Chromosome != null)
                {
                    play2048Chromosome.Score = new Random(0).NextDouble();
                    return play2048Chromosome.Score - 1000000f;
                }
                else
                {
                    return 0.0f;
                }
            }
        }
        public class CustomPopulation: Population
        {
            public CustomPopulation(int minSize, int maxSize, IChromosome chromosome) : base(minSize,maxSize, chromosome)
            {
            }

            public override void CreateInitialGeneration()
            {
                base.CreateInitialGeneration();
            }

        }

        public class Play2048Chromosome : ChromosomeBase
        {
            public int PatternLength { get; set; }
            // Change the argument value passed to base construtor to change the length 
            // of your chromosome.
            public double Score { get; set; }
            public List<string> Choices { get; set; }

            public Play2048Chromosome(int patternLength, List<string> choices)
                : base(patternLength)
            {
                PatternLength = patternLength;
                Choices = choices;
                CreateGenes();
                //Score = new Random(0).NextDouble();
            }

            public override Gene GenerateGene(int geneIndex)
            {
                return new Gene(Choices.PickRandom());
            }

            public override IChromosome CreateNew()
            {
                return new Play2048Chromosome(PatternLength, Choices);
            }

        }
    }
}
