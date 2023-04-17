using UnityEngine;
using Utility;

namespace Managers
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : Manager
    {
        [SerializeField] private LevelSO m_currentLevel;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<GameManager>(this);
            Application.targetFrameRate = 60;
        }

        public Vector2Int GetCurrentBoardSize()
        {
            return m_currentLevel.BoardSize;
        }

        public ProbabilityData GetItemProbabilities()
        {
            return m_currentLevel.Probabilities;
        }
    }
}
