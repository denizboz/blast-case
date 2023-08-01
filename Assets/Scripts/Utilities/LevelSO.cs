using System;
using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "Level_00", menuName = "New Level")]
    public class LevelSO : ScriptableObject
    {
        public Vector2Int BoardSize;
        public int NumberOfMoves = 50;

        public ProbabilityDistributionSO ProbDistribution;
        public Goal[] Goals;

        private const int maxNumberOfGoals = 3;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Goals.Length <= maxNumberOfGoals)
                return;
            
            var goals = new Goal[maxNumberOfGoals];
            
            for (var i = 0; i < goals.Length; i++)
                goals[i] = Goals[i];

            Goals = goals;
        }
#endif
    }

    [Serializable]
    public struct Goal
    {
        public Item Item;
        public int Target;
    }
}
