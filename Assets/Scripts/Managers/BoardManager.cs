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

        private static ItemPooler itemPooler;
        
        private static Item[,] itemsOnBoard;

        private ProbabilityData m_probabilities;

        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
            
            GameEvents.AddListener(BoardEvent.ItemTapped, OnItemTap);
        }

        private void OnEnable()
        {
            var gameManager = dependencyContainer.Resolve<GameManager>();
            var boardSize = gameManager.GetCurrentBoardSize();
            
            ResizeBorders(boardSize);
            FillItems(boardSize);
            
            SetProbabilitiesForItemFall();
        }

        private static void OnItemTap<T>(T item) where T : Item
        {
            if (item is Cube cube)
            {
                TryDestroyNeighbouringCubes(cube);
            }
            else if (item is Rocket rocket)
            {
                rocket.InitiateAction();
            }
        }

        private static void TryDestroyNeighbouringCubes(Cube cube)
        {
            var type = cube.Type;

            var posX = cube.Position.x;
            var posY = cube.Position.y;

            var left = posX != 0 ? itemsOnBoard[posX - 1, posY] : null;
            var right = posX != 8 ? itemsOnBoard[posX + 1, posY] : null;
            var up = posY != 8 ? itemsOnBoard[posX, posY + 1] : null;
            var down = posY != 0 ? itemsOnBoard[posX, posY - 1] : null;

            bool hasNeighbor = false;
            
            if (left is Cube cube1)
            {
                if (cube1.Type == type)
                {
                    hasNeighbor = true;
                    itemPooler.Return(cube1);
                }
            }
            
            if (right is Cube cube2)
            {
                if (cube2.Type == type)
                {
                    hasNeighbor = true;
                    itemPooler.Return(cube2);
                }
            }
            
            if (up is Cube cube3)
            {
                if (cube3.Type == type)
                {
                    hasNeighbor = true;
                    itemPooler.Return(cube3);
                }
            }
            
            if (down is Cube cube4)
            {
                if (cube4.Type == type)
                {
                    hasNeighbor = true;
                    itemPooler.Return(cube4);
                }
            }
            
            if (hasNeighbor)
                itemPooler.Return(cube);
        }
        
        private void SetProbabilitiesForItemFall()
        {
            var gameManager = dependencyContainer.Resolve<GameManager>();
            m_probabilities = gameManager.GetItemProbabilities();
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
            itemsOnBoard = new Item[9, 9];
            
            itemPooler = dependencyContainer.Resolve<ItemPooler>();
            var gridManager = dependencyContainer.Resolve<GridManager>();

            var range1 = Utilities.GetMidRange(boardSize.x);
            var range2 = Utilities.GetMidRange(boardSize.y);
            
            for (int i = range1.Start; i < range1.End; i++)
            {
                for (int j = range2.Start; j < range2.End; j++)
                {
                    var cube = itemPooler.Get<Cube>();
                    
                    int randInt = Random.Range(0, 5);
                    var type = (CubeType)randInt;

                    var sprite = m_spriteContainer.GetSprite(SpriteType.Cube, type);
                    
                    cube.SetType(type);
                    cube.SetSprite(sprite);
                    
                    cube.SetPositionAndSorting(i, j);
                    cube.transform.position = gridManager.GetWorldPosition(i, j);

                    itemsOnBoard[i, j] = cube;
                }
            }
        }
    }
}
