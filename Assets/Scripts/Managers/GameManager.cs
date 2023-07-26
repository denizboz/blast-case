using System.Collections.Generic;
using UnityEngine;
using Board;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour, IDependency
    {
        [SerializeField] private LevelSO m_currentLevel;

        private UIManager m_uiManager;
        
        private readonly Dictionary<GoalType, int> m_levelGoalsDictionary = new Dictionary<GoalType, int>();

        private int m_moveCount;

        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            m_uiManager = DI.Resolve<UIManager>();
            
            GameEventSystem.AddListener<MoveMadeEvent>(UpdateMoveCount);
            GameEventSystem.AddListener<ItemDestroyedEvent>(OnItemDestroyed);
            
            SetGoals();
        }

        private void Start()
        {
            m_uiManager.UpdateMoveCount(m_moveCount);
        }

        private void SetGoals()
        {
            var goals = m_currentLevel.Goals;

            for (var i = 0; i < goals.Length; i++)
            {
                m_levelGoalsDictionary.Add(goals[i].GoalType, goals[i].Target);
            }

            m_moveCount = m_currentLevel.NumberOfMoves;
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

        private void OnItemDestroyed(object item)
        {
            switch (item)
            {
                case Cube cube:
                    OnCubeDestroyed(cube);
                    break;
                case Balloon balloon:
                    OnBalloonDestroyed(balloon);
                    break;
                case Duck duck:
                    OnDuckDestroyed(duck);
                    break;
            }
        }
        
        private void OnCubeDestroyed(Cube cube)
        {
            var cubeType = cube.Type;
            var goalType = (GoalType)cubeType;

            if (!m_levelGoalsDictionary.ContainsKey(goalType))
                return;
            
            if (m_levelGoalsDictionary[goalType] == 0)
                return;
            
            m_levelGoalsDictionary[goalType]--;
            m_uiManager.UpdateGoal(goalType, m_levelGoalsDictionary[goalType]);
            
            CheckForSuccess();
        }

        private void OnBalloonDestroyed(Balloon balloon)
        {
            var goalType = GoalType.Balloon;
            
            if (!m_levelGoalsDictionary.ContainsKey(goalType))
                return;
            
            if (m_levelGoalsDictionary[goalType] == 0)
                return;

            m_levelGoalsDictionary[goalType]--;
            m_uiManager.UpdateGoal(goalType, m_levelGoalsDictionary[goalType]);
            
            CheckForSuccess();
        }

        private void OnDuckDestroyed(Duck duck)
        {
            var goalType = GoalType.Duck;
            
            if (!m_levelGoalsDictionary.ContainsKey(goalType))
                return;
            
            if (m_levelGoalsDictionary[goalType] == 0)
                return;

            m_levelGoalsDictionary[goalType]--;
            m_uiManager.UpdateGoal(goalType, m_levelGoalsDictionary[goalType]);
            
            CheckForSuccess();
        }

        private void CheckForSuccess()
        {
            bool isComplete = true;

            foreach (var pair in m_levelGoalsDictionary)
            {
                isComplete = isComplete && pair.Value == 0;
            }
            
            if (isComplete)
                GameEventSystem.Invoke<GameWonEvent>();
        }

        private void UpdateMoveCount(object obj)
        {
            m_moveCount--;
            
            m_uiManager.UpdateMoveCount(m_moveCount);
            
            if (m_moveCount < 1)
                GameEventSystem.Invoke<GameLostEvent>();
        }
    }
}
