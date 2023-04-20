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
        
        private static Item[,] itemsOnBoard;
        
        private static List<Cube> sameColoredCubes;

        private static ItemPooler itemPooler;
        private static GridManager gridManager;
        private static ItemSpawner itemSpawner;
        private static InputManager inputManager;
        
        private static ProbabilityData m_probabilities;

        private static int bottom, top;
        public const int MaxSize = 9;

        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
            
            GameEvents.AddListener(BoardEvent.ItemTapped, OnItemTap);
        }

        private void Start()
        {
            var gameManager = dependencyContainer.Resolve<GameManager>();
            var boardSize = gameManager.GetCurrentBoardSize();
            
            itemPooler = dependencyContainer.Resolve<ItemPooler>();
            gridManager = dependencyContainer.Resolve<GridManager>();
            itemSpawner = dependencyContainer.Resolve<ItemSpawner>();
            inputManager = dependencyContainer.Resolve<InputManager>();
            
            ResizeBorders(boardSize);
            FillItems(boardSize);
            
            SetFallProbabilities();
        }

        private void OnItemTap<T>(T item) where T : Item
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

        private void DestroyNeighbouringCubes(Cube tappedCube)
        {
            sameColoredCubes = new List<Cube>();
            FindSameColoredNeighbours(tappedCube);

            if (sameColoredCubes.Count < 2)
                return;

            //inputManager.EnableInput(false);
            
            foreach (var cube in sameColoredCubes)
            {
                RemoveItemFromBoard(cube);
                itemPooler.Return(cube);
                
                GameEvents.Invoke(BoardEvent.CubeDestroyed, cube);
            }

            MakeItemsFall();
            SpawnNewItems();
        }
        
        private static void FindSameColoredNeighbours(Cube cube)
        {
            if (sameColoredCubes.Contains(cube))
                return;
            
            sameColoredCubes.Add(cube);
            
            var posX = cube.Position.x;
            var posY = cube.Position.y;

            const int n = MaxSize - 1;
            var adjacentItems = new Item[4]
            {
                posX != 0 ? itemsOnBoard[posX - 1, posY] : null,
                posX != n ? itemsOnBoard[posX + 1, posY] : null,
                posY != 0 ? itemsOnBoard[posX, posY - 1] : null,
                posY != n ? itemsOnBoard[posX, posY + 1] : null
            };
            
            foreach (var item in adjacentItems)
            {
                if (item is not Cube nearCube)
                    continue;
                
                if (nearCube.Type == cube.Type)
                    FindSameColoredNeighbours(nearCube);
            }
        }

        private void MakeItemsFall()
        {
            var columnIndices = sameColoredCubes.Select(cube => cube.Position.y).Distinct();

            foreach (var columnIndex in columnIndices)
            {
                for (int j = bottom + 1; j < top + 1; j++)
                {
                    var itemsInColumn = itemsOnBoard.GetColumn(columnIndex);
                    
                    var item1 = itemsInColumn[j];

                    if (!item1)
                        continue;

                    for (int k = bottom; k < j; k++)
                    {
                        var item2 = itemsInColumn[k];

                        if (item2)
                            continue;

                        UpdateItemPos(item1, k, item1.Position.y);
                        var emptyPos = gridManager.GetWorldPosition(k, item1.Position.y);
                        item1.MoveTo(emptyPos);

                        break;
                    }
                }
                
                //SpawnItemsToColumn(columnIndex, highestRow);
            }
        }

        private void SpawnItemsToColumn(int columnIndex, int highestRow)
        {
            Utilities.Debug(columnIndex, highestRow, top);
            
            for (int i = highestRow + 1; i < top + 1; i++)
            {
                var finalPos = new Vector2Int(i, columnIndex);
                
                var rand = Random.value;

                if (rand < m_probabilities.Cube)
                {
                    var cube = (Cube)itemSpawner.Spawn<Cube>(finalPos);
                    var type = (CubeType)Random.Range(0, Cube.VarietySize);

                    cube.SetType(type);
                    cube.SetSprite(m_spriteContainer.GetSprite(SpriteType.Cube, type));
                }
                else if (rand < m_probabilities.Cube + m_probabilities.Balloon)
                {
                    var balloon = itemSpawner.Spawn<Balloon>(finalPos);
                    balloon.SetSprite(m_spriteContainer.GetSprite(SpriteType.Balloon));
                }
                else
                {
                    var duck = itemSpawner.Spawn<Duck>(finalPos);
                    duck.SetSprite(m_spriteContainer.GetSprite(SpriteType.Duck));
                }
            }
        }
        
        private void SpawnNewItems()
        {
            var groups = sameColoredCubes.GroupBy(cube => cube.Position.y);
            
            foreach (var group in groups)
            {
                var count = group.Count();
                var column = group.Key;

                for (int i =  top - count + 1; i < top + 1; i++)
                {
                    var finalPos = new Vector2Int(i, column);
                    
                    var rand = Random.value;

                    if (rand < m_probabilities.Cube)
                    {
                        var cube = (Cube)itemSpawner.Spawn<Cube>(finalPos);
                        var type = (CubeType)Random.Range(0, Cube.VarietySize);

                        cube.SetType(type);
                        cube.SetSprite(m_spriteContainer.GetSprite(SpriteType.Cube, type));
                    }
                    else if (rand < m_probabilities.Cube + m_probabilities.Balloon)
                    {
                        var balloon = itemSpawner.Spawn<Balloon>(finalPos);
                        balloon.SetSprite(m_spriteContainer.GetSprite(SpriteType.Balloon));
                    }
                    else
                    {
                        var duck = itemSpawner.Spawn<Duck>(finalPos);
                        duck.SetSprite(m_spriteContainer.GetSprite(SpriteType.Duck));
                    }
                }
            }
        }
        
        public void AddItemToBoard(Item item)
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
            
            item.SetGridPositionAndSorting(new Vector2Int(x, y));
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
            itemsOnBoard = new Item[MaxSize, MaxSize];
            
            var range1 = Utilities.GetMidRange(boardSize.x);
            var range2 = Utilities.GetMidRange(boardSize.y);

            bottom = range1.Start;
            top = range1.End;
            
            for (int i = range1.Start; i < range1.End + 1; i++)
            {
                for (int j = range2.Start; j < range2.End + 1; j++)
                {
                    var cube = itemPooler.Get<Cube>();
                    
                    int randInt = Random.Range(0, Cube.VarietySize);
                    var type = (CubeType)randInt;

                    var sprite = m_spriteContainer.GetSprite(SpriteType.Cube, type);
                    
                    cube.SetType(type);
                    cube.SetSprite(sprite);
                    
                    cube.SetGridPositionAndSorting(new Vector2Int(i, j));
                    cube.transform.position = gridManager.GetWorldPosition(i, j);

                    itemsOnBoard[i, j] = cube;
                }
            }
        }
    }
}
