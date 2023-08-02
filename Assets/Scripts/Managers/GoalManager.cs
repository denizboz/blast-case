using System;
using System.Collections.Generic;
using Board;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class GoalManager : MonoBehaviour, IDependency
    {
        private readonly Dictionary<Type, int> m_levelGoalsDictionary = new Dictionary<Type, int>();

        private UIManager m_uiManager;
        private int m_moveCount;
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            m_uiManager = DI.Resolve<UIManager>();
            
            GameEventSystem.AddListener<LevelLoadedEvent>(SetGoals);
            GameEventSystem.AddListener<MoveMadeEvent>(UpdateMoveCount);
            GameEventSystem.AddListener<ItemDestroyedEvent>(OnItemDestroyed);
        }

        private void SetGoals(object levelSO)
        {
            var level = (LevelSO)levelSO;
            var goals = level.Goals;
            
            m_moveCount = level.NumberOfMoves;

            for (var i = 0; i < goals.Length; i++)
            {
                m_levelGoalsDictionary.Add(goals[i].Item.Type, goals[i].Target);
            }
        }
        
        private void OnItemDestroyed(object item)
        {
            var destroyedItem = (Item)item;
            var goalType = destroyedItem.Type;
            
            if (!m_levelGoalsDictionary.ContainsKey(goalType))
                return;
            
            if (m_levelGoalsDictionary[goalType] == 0)
                return;
            
            m_levelGoalsDictionary[goalType]--;
            m_uiManager.UpdateGoalUI(goalType, m_levelGoalsDictionary[goalType]);
            
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
            
            m_uiManager.UpdateMovesUI(m_moveCount);
            
            if (m_moveCount < 1)
                GameEventSystem.Invoke<GameLostEvent>();
        }
    }
}