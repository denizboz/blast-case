using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-50)]
    public class GridManager : Manager
    {
        private Transform[,] m_gridPoints;
        
        private const float boardWidth = 10.9f;
        private const float itemWidth = 1.19f;
        private const float offset = 0.1f;

        protected override void Awake()
        {
            dependencyContainer.Bind<GridManager>(this);
            
            CreateGrid();
        }

        private void CreateGrid()
        {
            var pointsParent = new GameObject("GridPoints").transform;
            pointsParent.parent = transform;

            var gameManager = dependencyContainer.Resolve<GameManager>();
            var boardSize = gameManager.GetCurrentBoardSize();

            m_gridPoints = new Transform[boardSize.x, boardSize.y];

            for (int i = 0; i < boardSize.x; i++)
            {
                for (int j = 0; j < boardSize.y; j++)
                {
                    var x = offset + (j + 0.5f) * itemWidth - boardWidth / 2f;
                    var y = offset + (i + 0.5f) * itemWidth - boardWidth / 2f;

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
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return m_gridPoints[x, y].position;
        }
    }
}
