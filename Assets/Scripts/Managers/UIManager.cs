using System;
using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using TMPro;
using UI;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class UIManager : MonoBehaviour, IDependency
    {
        [SerializeField] private TextMeshProUGUI m_moveCountUI;
        [SerializeField] private GameObject m_successPanel;
        [SerializeField] private GameObject m_failurePanel;
        [SerializeField] private RectTransform m_goalsArea;
        [SerializeField] private GoalUI[] m_goalFields;

        private List<GoalUI> m_activeGoalFields;


        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            GameEventSystem.AddListener<LevelLoadedEvent>(LoadGoalUIs);
            GameEventSystem.AddListener<GameWonEvent>(ShowSuccessUI);
            GameEventSystem.AddListener<GameLostEvent>(ShowFailureUI);
        }

        public void UpdateGoalUI(Type type, int count)
        {
            foreach (var field in m_activeGoalFields)
            {
                if (field.Item.Type == type)
                    field.UpdateText(count);
            }
        }
        
        public void UpdateMovesUI(int count)
        {
            m_moveCountUI.text = count.ToString();
        }
        
        private void LoadGoalUIs(object levelSO)
        {
            var level = (LevelSO)levelSO;
            var goals = level.Goals;
            
            UpdateMovesUI(level.NumberOfMoves);

            var areaWidth = m_goalsArea.sizeDelta.x;
            var fieldWidth = GoalUI.Width;

            m_activeGoalFields = new List<GoalUI>(goals.Length);
            
            for (var i = 0; i < goals.Length; i++)
            {
                var goalItem = goals[i].Item;

                var goalField = m_goalFields[i];
                
                goalField.Set(goalItem, goalItem.Sprite);
                goalField.UpdateText(goals[i].Target);
                
                m_activeGoalFields.Add(goalField);
            }
            
            if (goals.Length == 1)
            {
                m_goalFields[0].SetPositionWithX(0f);
            }
            else if (goals.Length == 2)
            {
                var gap = (areaWidth - 2f * fieldWidth) / 3f;
                var x = fieldWidth / 2f + gap / 2f;
                
                m_goalFields[0].SetPositionWithX(-x);
                m_goalFields[1].SetPositionWithX(x);
            }
            else if (goals.Length == 3)
            {
                var gap = (areaWidth - 3f * fieldWidth) / 2f;
                var x = fieldWidth + gap;
                
                m_goalFields[0].SetPositionWithX(-x);
                m_goalFields[1].SetPositionWithX(0f);
                m_goalFields[2].SetPositionWithX(x);
            }
        }

        private void ShowSuccessUI(object obj)
        {
            m_successPanel.SetActive(true);
        }

        private void ShowFailureUI(object obj)
        {
            m_failurePanel.SetActive(true);
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelLoadedEvent>(LoadGoalUIs);
            GameEventSystem.RemoveListener<GameWonEvent>(ShowSuccessUI);
            GameEventSystem.RemoveListener<GameLostEvent>(ShowFailureUI);
        }
    }
}
