using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    public enum GoalType { YellowCube, RedCube, BlueCube, GreenCube, PurpleCube, Balloon, Duck }

    [CreateAssetMenu(fileName = "Level_00", menuName = "New Level")]
    public class LevelSO : ScriptableObject
    {
        public Vector2Int BoardSize = new Vector2Int(9, 9);
        public int NumberOfMoves = 50;

        public Goal[] Goals;
    }

    [Serializable]
    public struct Goal
    {
        public GoalType Type;
        public int Target;
    }
}
