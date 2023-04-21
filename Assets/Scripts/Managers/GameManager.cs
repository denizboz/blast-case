using System.Collections.Generic;
using Board;
using UnityEngine;
using Utility;

namespace Managers
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : Manager
    {
        [SerializeField] private LevelSO m_currentLevel;

        private static UIManager uiManager;
        
        private static readonly Dictionary<GoalType, int> m_levelGoalsDictionary = new Dictionary<GoalType, int>();

        private static int moveCount;

        protected override void Awake()
        {
            dependencyContainer.Bind<GameManager>(this);
            Application.targetFrameRate = 60;
            
            GameEvents.AddListener(BoardEvent.CubeDestroyed, OnCubeDestroyed);
            GameEvents.AddListener(BoardEvent.BalloonDestroyed, OnBalloonDestroyed);
            GameEvents.AddListener(BoardEvent.DuckDestroyed, OnDuckDestroyed);
            
            GameEvents.AddListener(CoreEvent.MoveMade, UpdateMoveCount);
            
            SetGoals();
        }

        private void Start()
        {
            uiManager = dependencyContainer.Resolve<UIManager>();
            uiManager.UpdateMoveCount(moveCount);
        }

        private void SetGoals()
        {
            var goals = m_currentLevel.Goals;

            for (var i = 0; i < goals.Length; i++)
            {
                m_levelGoalsDictionary.Add(goals[i].GoalType, goals[i].Target);
            }

            moveCount = m_currentLevel.NumberOfMoves;
        }

        public Vector2Int GetCurrentBoardSize()
        {
            return m_currentLevel.BoardSize;
        }

        public ProbabilityData GetItemProbabilities()
        {
            return m_currentLevel.Probabilities;
        }

        public Goal[] GetGoals()
        {
            return m_currentLevel.Goals;
        }

        private static void OnCubeDestroyed(Item cube)
        {
            var cubeType = ((Cube)cube).Type;
            var goalType = (GoalType)cubeType;

            if (!m_levelGoalsDictionary.ContainsKey(goalType))
                return;
            
            if (m_levelGoalsDictionary[goalType] == 0)
                return;
            
            m_levelGoalsDictionary[goalType]--;
            
            CheckForSuccess();
        }

        private static void OnBalloonDestroyed(Item balloon)
        {
            if (!m_levelGoalsDictionary.ContainsKey(GoalType.Balloon))
                return;
            
            if (m_levelGoalsDictionary[GoalType.Balloon] == 0)
                return;

            m_levelGoalsDictionary[GoalType.Balloon]--;
            
            CheckForSuccess();
        }

        private static void OnDuckDestroyed(Item duck)
        {
            if (!m_levelGoalsDictionary.ContainsKey(GoalType.Duck))
                return;
            
            if (m_levelGoalsDictionary[GoalType.Duck] == 0)
                return;

            m_levelGoalsDictionary[GoalType.Duck]--;
            
            CheckForSuccess();
        }

        private static void CheckForSuccess()
        {
            bool isComplete = true;

            foreach (var pair in m_levelGoalsDictionary)
            {
                isComplete = isComplete && pair.Value == 0;
            }
            
            if (isComplete)
                GameEvents.Invoke(CoreEvent.GameWon);
        }

        private static void UpdateMoveCount()
        {
            moveCount--;
            
            uiManager.UpdateMoveCount(moveCount);
            
            if (moveCount < 1)
                GameEvents.Invoke(CoreEvent.GameLost);
        }
    }
}
