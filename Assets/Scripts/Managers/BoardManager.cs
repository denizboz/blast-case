using System.Collections.Generic;
using System.Linq;
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

        private static List<Cube> sameColoredCubes;

        private static ItemPooler itemPooler;
        private static GridManager gridManager;
        private static InputManager inputManager;
        
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
            
            itemPooler = dependencyContainer.Resolve<ItemPooler>();
            gridManager = dependencyContainer.Resolve<GridManager>();
            inputManager = dependencyContainer.Resolve<InputManager>();
            
            ResizeBorders(boardSize);
            FillItems(boardSize);
            
            SetFallProbabilities();
        }

        private static void OnItemTap<T>(T item) where T : Item
        {
            if (item is Cube cube)
            {
                DestroyNeighbouringCubes(cube);
            }
            else if (item is Rocket rocket)
            {
                rocket.InitiateAction();
            }
        }

        private static void DestroyNeighbouringCubes(Cube tappedCube)
        {
            sameColoredCubes = new List<Cube>();
            FindSameColoredNeighbours(tappedCube);

            if (sameColoredCubes.Count < 2)
                return;

            //inputManager.EnableInput(false);
            
            foreach (var cube in sameColoredCubes)
            {
                var pos = cube.Position;

                RemoveItemFromBoard(cube);
                itemPooler.Return(cube);
                
                GameEvents.Invoke(BoardEvent.CubeDestroyed, cube);
            }

            MakeItemsFall();
        }
        
        private static void FindSameColoredNeighbours(Cube cube)
        {
            if (sameColoredCubes.Contains(cube))
                return;
            
            sameColoredCubes.Add(cube);
            
            var posX = cube.Position.x;
            var posY = cube.Position.y;

            var adjacentItems = new Item[4]
            {
                posX != 0 ? itemsOnBoard[posX - 1, posY] : null,
                posX != 8 ? itemsOnBoard[posX + 1, posY] : null,
                posY != 8 ? itemsOnBoard[posX, posY + 1] : null,
                posY != 0 ? itemsOnBoard[posX, posY - 1] : null
            };
            
            foreach (var item in adjacentItems)
            {
                if (item is not Cube nearCube)
                    continue;
                
                if (nearCube.Type == cube.Type)
                    FindSameColoredNeighbours(nearCube);
            }
        }

        private static void MakeItemsFall()
        {
            var columns = sameColoredCubes.Select(cube => cube.Position.y).Distinct();

            foreach (var column in columns)
            {
                for (int j = 1; j < 9; j++)
                {
                    var itemsInColumn = itemsOnBoard.GetColumn(column);
                    
                    var item1 = itemsInColumn[j];

                    if (!item1)
                        continue;
                    
                    for (int k = 0; k < j; k++)
                    {
                        var item2 = itemsInColumn[k];

                        if (item2)
                            continue;

                        UpdateItemPos(item1, k, item1.Position.y);
                        var emptyPos = gridManager.GetWorldPosition(k, item1.Position.y);
                        item1.FallTo(emptyPos);
                        
                        break;
                    }
                }
            }
        }
        
        private static void AddItemToBoard(Item item)
        {
            var pos = item.Position;
            itemsOnBoard[pos.x, pos.y] = item;
        }

        private static void RemoveItemFromBoard(Item item)
        {
            var pos = item.Position;
            itemsOnBoard[pos.x, pos.y] = null;
        }

        private static void UpdateItemPos(Item item, int x, int y)
        {
            var oldPos = item.Position;
            itemsOnBoard[oldPos.x, oldPos.y] = null;
            
            item.SetPositionAndSorting(x, y);
            itemsOnBoard[x, y] = item;
        }
        
        private void SetFallProbabilities()
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
                    
                    AddItemToBoard(cube);

                    // for debug - remove before build:
                    cube.name = $"cube_{i}{j}";
                    // :for debug - remove before build
                }
            }
        }
    }
}
