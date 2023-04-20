using UnityEngine;
using Utility;

namespace Managers
{
    [DefaultExecutionOrder(-50)]
    public class GridManager : Manager
    {
        private readonly Transform[,] m_gridPoints = new Transform[gridSize, gridSize];

        private const int gridSize = BoardManager.MaxSize;
        
        private const float boardWidth = 10.9f;
        private const float itemWidth = 1.19f;
        private const float borderThickness = 0.1f;

        private const float spawnHeight = 10f;

        protected override void Awake()
        {
            dependencyContainer.Bind<GridManager>(this);
            
            CreateGrid();
        }

        private void CreateGrid()
        {
            var pointsParent = new GameObject("GridPoints").transform;
            pointsParent.parent = transform;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
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

            var gameManager = dependencyContainer.Resolve<GameManager>();
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
        
        public Vector3 GetSpawnPosition(int column)
        {
            return m_gridPoints[0, column].position.WithY((spawnHeight));
        }
    }
}
