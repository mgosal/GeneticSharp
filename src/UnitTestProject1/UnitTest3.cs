using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ExtensionLibrary;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void TestMethod1()
        {
            GA.GetSingleton().Init(4, new List<string>() { "up", "down", "left", "right" }, 3, 4);
            var p0 = GA.GetSingleton().GetPatternsKeys();
            Console.WriteLine("GA running...");
            GA.GetSingleton().EvolveNextGeneration();
            var p1 = GA.GetSingleton().GetPatternsKeys();
        }
    }




    public class GA
    {
        private GA()
        {
            // Prevent outside instantiation
        }

        private static readonly GA _singleton = new GA();
        private EliteSelection _eliteSelection;
        private TwoPointCrossover _twoPointCrossover;
        private PartialShuffleMutation _partialShuffleMutation;
        private ClosestToMillion _closestToMillion;
        private Play2048Chromosome _play2048Chromosome;
        private Population _population;
        private GeneticSharp.Domain.GeneticAlgorithm _geneticAlgorithm;

        public EliteSelection EliteSelection { get => _eliteSelection; set => _eliteSelection = value; }

        public static GA GetSingleton()
        {
            return _singleton;
        }

        public void Init(int patternLength, List<string> choices, int populationSize, int maxGenerations)
        {
            EliteSelection = new EliteSelection();
            _twoPointCrossover = new TwoPointCrossover();
            _partialShuffleMutation = new PartialShuffleMutation();
            _closestToMillion = new ClosestToMillion();

            _play2048Chromosome = new Play2048Chromosome(patternLength, choices);
            //var bestPattern = BestPattern.Get(context);
            _population = new Population(populationSize, populationSize, _play2048Chromosome);

            _geneticAlgorithm = new GeneticSharp.Domain.GeneticAlgorithm(_population, _closestToMillion, EliteSelection,
                _twoPointCrossover, _partialShuffleMutation);
            _geneticAlgorithm.Termination = new GenerationNumberTermination(maxGenerations);
            _geneticAlgorithm.Init();
        }

        public void SetScore(string p, int s)
        {
            var c = (Play2048Chromosome)_geneticAlgorithm.Population.Generations[_geneticAlgorithm.GenerationsNumber - 1].Chromosomes.Where(x => (x as Play2048Chromosome).PatternKeys == p);
            c.Score = s;
        }

        public List<string> GetPatternsKeys()
        {
            var r = new List<string>();
            foreach (var item in _geneticAlgorithm.Population.Generations[_geneticAlgorithm.GenerationsNumber - 1].Chromosomes)
            {
                r.Add((item as Play2048Chromosome).PatternKeys);
            }
            return r;
        }

        public void EvolveNextGeneration()
        {
            _geneticAlgorithm.EvolveNextGeneration();
        }
    }

    public class Play2048Chromosome : ChromosomeBase
    {
        public int PatternLength { get; set; }
        // Change the argument value passed to base construtor to change the length 
        // of your chromosome.
        public double Score { get; set; }
        public List<string> Choices { get; set; }
        public string Pattern
        {
            get
            {
                StringBuilder s = new StringBuilder(PatternLength);
                for (int i = 0; i < GetGenes().Length; i++)
                {
                    s.Append(ExtensionLibrary.LeftExtension.Left(GetGene(i).ToString(), 1));
                }
                return s.ToString();
            }
        }

        public string PatternKeys
        {
            get
            {
                StringBuilder keys = new StringBuilder();
                for (int i = 0; i < GetGenes().Length; i++)
                {
                    keys.Append("[k(" + GetGene(i) + ")]");
                }
                return keys.ToString();
            }
        }

        public Play2048Chromosome(int patternLength, List<string> choices)
            : base(patternLength)
        {
            PatternLength = patternLength;
            Choices = choices;
            //Score = new Random(0).NextDouble();
            CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(PickRandomExtension.PickRandom(Choices));
        }

        public override IChromosome CreateNew()
        {
            return new Play2048Chromosome(PatternLength, Choices);
        }
    }

    public class ClosestToMillion : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {

            var timeoutTime = DateTime.Now.AddMinutes(10);
            // Evaluate the fitness of chromosome.
            var play2048Chromosome = chromosome as Play2048Chromosome;
            if (play2048Chromosome != null)
                return play2048Chromosome.Score - 1000000f;
            else
            {
                return 0.0f;
            }
        }
    }

}
