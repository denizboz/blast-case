using TMPro;
using UI;
using UnityEngine;
using Utility;

namespace Managers
{
    public class UIManager : Manager
    {
        [SerializeField] private SpriteContainer m_spriteContainer;
        [SerializeField] private TextMeshProUGUI m_moveCountUI;
        [SerializeField] private GameObject m_successPanel;
        [SerializeField] private GameObject m_failurePanel;
        [SerializeField] private RectTransform m_goalsArea;
        [SerializeField] private GoalUI[] m_goalFields;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<UIManager>(this);
            
            GameEvents.AddListener(CoreEvent.BoardLoaded, LoadGoalUIs);
            GameEvents.AddListener(CoreEvent.GameWon, ShowSuccessUI);
            GameEvents.AddListener(CoreEvent.GameLost, ShowFailureUI);
        }

        public void UpdateMoveCount(int count)
        {
            m_moveCountUI.text = count.ToString();
        }
        
        private void LoadGoalUIs()
        {
            var gameManager = dependencyContainer.Resolve<GameManager>();
            
            var goals = gameManager.GetGoals();

            var areaWidth = m_goalsArea.sizeDelta.x;
            var fieldWidth = m_goalFields[0].GetWidth();

            for (var i = 0; i < goals.Length; i++)
            {
                
            }
            
            if (goals.Length == 1)
            {
                m_goalFields[0].SetPositionWithX(0);
            }
        }

        private void ShowSuccessUI()
        {
            m_successPanel.SetActive(true);
        }

        private void ShowFailureUI()
        {
            m_failurePanel.SetActive(true);
        }
    }
}
