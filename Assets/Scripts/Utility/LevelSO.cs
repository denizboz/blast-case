using System;
using UnityEngine;
using Managers;
using Board;

namespace Utility
{
    public enum GoalType { YellowCube, RedCube, BlueCube, GreenCube, PurpleCube, Balloon, Duck }
    
    [CreateAssetMenu(fileName = "Level_00", menuName = "New Level")]
    public class LevelSO : ScriptableObject
    {
        public Vector2Int BoardSize = new Vector2Int(BoardManager.MaxSize, BoardManager.MaxSize);
        public int NumberOfMoves = 50;

        public ProbabilityData Probabilities;
        
        public Goal[] Goals;

        public const int MaxNumberOfGoals = 3;

        private void OnValidate()
        {
            LimitBoardSize();
            LimitGoals();
            NormalizeProbabilities();
        }

        private void LimitBoardSize()
        {
            var x = Math.Clamp(BoardSize.x, BoardManager.MinSize, BoardManager.MaxSize);
            var y = Math.Clamp(BoardSize.y, BoardManager.MinSize, BoardManager.MaxSize);

            BoardSize = new Vector2Int(x, y);
        }

        private void LimitGoals()
        {
            if (Goals.Length <= MaxNumberOfGoals)
                return;
            
            var goals = new Goal[MaxNumberOfGoals];
            
            for (var i = 0; i < goals.Length; i++)
                goals[i] = Goals[i];

            Goals = goals;
        }
        
        private void NormalizeProbabilities()
        {
            Probabilities.Cube = Mathf.Clamp(Probabilities.Cube, 0f, 1f);
            Probabilities.Balloon = Mathf.Clamp(Probabilities.Balloon, 0f, 1f);
            Probabilities.Duck = Mathf.Clamp(Probabilities.Duck, 0f, 1f);

            var sum = Probabilities.GetSum();

            if (Math.Abs(sum - 1f) < 0.01f)
                return;

            var prob = 1f - Probabilities.Cube;
            Probabilities.Balloon = prob * 0.75f;
            Probabilities.Duck = prob * 0.25f;
        }
    }

    [Serializable]
    public struct Goal
    {
        public GoalType GoalType;
        public int Target;
    }

    [Serializable]
    public struct ProbabilityData
    {
        public float Cube;
        public float Balloon;
        public float Duck;

        public float GetSum()
        {
            return Cube + Balloon + Duck;
        }
    }
}
