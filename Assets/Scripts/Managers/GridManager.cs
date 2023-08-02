using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour, IDependency
    {
        [SerializeField] private SpriteRenderer m_borders;
        
        private Transform[,] m_gridPoints;

        private readonly int m_gridSize = BoardManager.MaxSize;
        
        private const float boardWidth = 10.9f;
        private const float itemWidth = 1.19f;
        private const float borderThickness = 0.1f;

        private const float spawnHeight = 10f;

        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            CreateGrid();
            GameEventSystem.AddListener<BoardLoadedEvent>(OnBoardLoaded);
        }

        private void CreateGrid()
        {
            m_gridPoints = new Transform[m_gridSize, m_gridSize];
            
            var pointsParent = new GameObject("GridPoints").transform;
            pointsParent.parent = transform;

            for (int i = 0; i < m_gridSize; i++)
            {
                for (int j = 0; j < m_gridSize; j++)
                {
                    var x = borderThickness + (j + 0.5f) * itemWidth - boardWidth / 2f;
                    var y = borderThickness + (i + 0.5f) * itemWidth - boardWidth / 2f;

                    var point = new GameObject($"point_{i.ToString()}")
                    {
                        transform =
                        {
                            parent = pointsParent,
                            localPosition = new Vector3(x, y, 0f)
                        }
                    };

                    m_gridPoints[i, j] = point.transform;
                }
            }

            var gameManager = DI.Resolve<GameManager>();
            var boardSize = gameManager.GetCurrentBoardSize();

            var offsetX = boardSize.y % 2 == 1 ? 0f : itemWidth / 2f;
            var offsetY = boardSize.x % 2 == 1 ? 0f : itemWidth / 2f;

            var offset = new Vector3(offsetX, offsetY, 0f);

            pointsParent.transform.localPosition += offset;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return m_gridPoints[x, y].position;
        }

        public Vector3 GetWorldPosition(Vector2Int gridPos)
        {
            return m_gridPoints[gridPos.x, gridPos.y].position;
        }
        
        public Vector3 GetSpawnPosition(Vector2Int gridPos)
        {
            return GetWorldPosition(gridPos).WithY(spawnHeight);
        }
        
        private void OnBoardLoaded(object boardSize)
        {
            ResizeBorders((Vector2Int)boardSize);
        }
        
        private void ResizeBorders(Vector2Int boardSize)
        {
            var borderSize = m_borders.size;

            var correctionX = Mathf.Lerp(0, 0.08f, (9f - boardSize.y) / 9f);
            var correctionY = Mathf.Lerp(0, 0.33f, (9f - boardSize.x) / 9f);
            
            var newX = (boardSize.y / 9f) * borderSize.x + correctionX;
            var newY = (boardSize.x / 9f) * borderSize.y + correctionY;

            m_borders.size = new Vector2(newX, newY);
        }
    }
}
