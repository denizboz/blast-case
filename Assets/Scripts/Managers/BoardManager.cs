using Board;
using Utility;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-30)]
    public class BoardManager : Manager
    {
        [SerializeField] private SpriteContainer m_spriteContainer;
        [SerializeField] private SpriteRenderer m_borders;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
        }

        private void OnEnable()
        {
            var gameManager = dependencyContainer.Resolve<GameManager>();
            var boardSize = gameManager.GetCurrentBoardSize();
            
            ResizeBorders(boardSize);
            FillItems(boardSize);
        }

        private void ResizeBorders(Vector2Int boardSize)
        {
            var size = m_borders.size;

            var newX = (boardSize.y / 9f) * size.x;
            var newY = (boardSize.x / 9f) * size.y;

            m_borders.size = new Vector2(newX, newY);
        }
        
        private void FillItems(Vector2Int boardSize)
        {
            var x = boardSize.x;
            var y = boardSize.y;

            var itemPooler = dependencyContainer.Resolve<ItemPooler>();
            var gridManager = dependencyContainer.Resolve<GridManager>();
            
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var cube = itemPooler.Get<Cube>();

                    var tuple = m_spriteContainer.GetRandomCube();
                    cube.SetType(tuple.Item1, tuple.Item2);
                    cube.SetPositionAndSorting(j, i);
                    
                    cube.transform.position = gridManager.GetWorldPosition(i, j);
                }
            }
        }
    }
}
