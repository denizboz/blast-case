using UnityEngine;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour, IDependency
    {
        [SerializeField] private LevelSO m_currentLevel;

        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            GameEventSystem.Invoke<LevelLoadedEvent>(m_currentLevel);
        }

        public Vector2Int GetCurrentBoardSize()
        {
            return m_currentLevel.BoardSize;
        }
    }
}
