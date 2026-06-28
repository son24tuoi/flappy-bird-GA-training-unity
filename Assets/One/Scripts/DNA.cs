using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace One.ML.GA.TappyPane
{
    [Serializable]
    public class DNA
    {
        [SerializeField] private List<int> genes = new List<int>();
        private int dnaLength = 0;
        private int maxValues = 0;

        public DNA(int length, int maxValues)
        {
            this.dnaLength = length;
            this.maxValues = maxValues;

            SetRandom();
        }

        public void SetRandom()
        {
            for (int i = 0; i < dnaLength; i++)
            {
                genes.Add(Random.Range(-maxValues, maxValues));
            }
        }

        public void SetInt(int pos, int value)
        {
            genes[pos] = value;
        }

        public void Combine(DNA dna1, DNA dna2)
        {
            for (int i = 0; i < dnaLength; i++)
            {
                genes[i] = Random.Range(0, 2) == 0 ? dna1.genes[i] : dna2.genes[i];
            }
        }

        public void Mutate()
        {
            genes[Random.Range(0, dnaLength)] = Random.Range(-maxValues, maxValues);
        }

        public int GetGene(int pos)
        {
            return genes[pos];
        }
    }
}
