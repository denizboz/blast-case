using Board;
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
        [SerializeField] private SpriteContainer m_spriteContainer;
        [SerializeField] private TextMeshProUGUI m_moveCountUI;
        [SerializeField] private GameObject m_successPanel;
        [SerializeField] private GameObject m_failurePanel;
        [SerializeField] private RectTransform m_goalsArea;
        [SerializeField] private GoalUI[] m_goalFields;
        
        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            GameEventSystem.AddListener<BoardLoadedEvent>(LoadGoalUIs);
            GameEventSystem.AddListener<GameWonEvent>(ShowSuccessUI);
            GameEventSystem.AddListener<GameLostEvent>(ShowFailureUI);
        }

        public void UpdateGoal(GoalType type, int count)
        {
            foreach (var field in m_goalFields)
            {
                if (field.Type == type)
                    field.UpdateText(count);
            }
        }
        
        public void UpdateMoveCount(int count)
        {
            m_moveCountUI.text = count.ToString();
        }
        
        private void LoadGoalUIs(object obj)
        {
            var gameManager = DI.Resolve<GameManager>();
            
            var goals = gameManager.GetGoals();

            var areaWidth = m_goalsArea.sizeDelta.x;
            var fieldWidth = GoalUI.Width;

            for (var i = 0; i < goals.Length; i++)
            {
                var goalType = goals[i].GoalType;

                Sprite sprite;

                if ((int)goalType < Cube.VarietySize)
                {
                    sprite = m_spriteContainer.GetSprite<Cube>((CubeType)goalType);
                }
                else if ((int)goalType == Cube.VarietySize)
                {
                    sprite = m_spriteContainer.GetSprite<Balloon>();
                }
                else
                {
                    sprite = m_spriteContainer.GetSprite<Duck>();
                }
                
                m_goalFields[i].Set(goalType, sprite);
                m_goalFields[i].UpdateText(goals[i].Target);
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
    }
}
