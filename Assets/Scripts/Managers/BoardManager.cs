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

            var correctionX = Mathf.Lerp(0, 0.08f, (9f - boardSize.y) / 9f);
            var correctionY = Mathf.Lerp(0, 0.33f, (9f - boardSize.x) / 9f);
            
            var newX = (boardSize.y / 9f) * size.x + correctionX;
            var newY = (boardSize.x / 9f) * size.y + correctionY;

            m_borders.size = new Vector2(newX, newY);
        }
        
        private void FillItems(Vector2Int boardSize)
        {
            var itemPooler = dependencyContainer.Resolve<ItemPooler>();
            var gridManager = dependencyContainer.Resolve<GridManager>();

            var range1 = Utilities.GetMidRange(boardSize.x);
            var range2 = Utilities.GetMidRange(boardSize.y);
            
            for (int i = range1.Start; i < range1.End; i++)
            {
                for (int j = range2.Start; j < range2.End; j++)
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
