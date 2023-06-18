using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Board;
using Events;
using Events.Implementations.Board;
using Events.Implementations.Core;
using Events.Implementations.Input;

namespace Managers
{
    [DefaultExecutionOrder(-10)]
    public class BoardManager : Manager
    {
        [SerializeField] private SpriteRenderer m_borders;
        
        private static Item[,] itemsOnBoard;

        private static List<Item> chainedItems;
        private static int chainedCubeCount;

        private static CubeType tappedCubeType;
        
        private static ItemPooler itemPooler;
        private static GridManager gridManager;
        private static ItemSpawner itemSpawner;

        private static Vector2Int boardSize;
        
        private static ProbabilityData m_probabilities;

        public static int Bottom, Top;

        public const int MinSize = 3;
        public const int MaxSize = 9;
        
        
        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
            
            GameEventSystem.AddListener<ItemTappedEvent>(OnItemTap);
            GameEventSystem.AddListener<DuckHitBottomEvent>(OnDuckHitBottom);
        }

        private void Start()
        {
            itemPooler = dependencyContainer.Resolve<ItemPooler>();
            gridManager = dependencyContainer.Resolve<GridManager>();
            itemSpawner = dependencyContainer.Resolve<ItemSpawner>();
            
            var gameManager = dependencyContainer.Resolve<GameManager>();
            boardSize = gameManager.GetCurrentBoardSize();
            
            FillItems();
            ResizeBorders();
            
            SetFallProbabilities();
        }

        private static void OnItemTap(object item)
        {
            ((ITappable)item).OnTap();
        }

        public static void DestroyNeighbouringItems(Cube tappedCube)
        {
            tappedCubeType = tappedCube.Type;
            
            chainedItems = new List<Item>();
            chainedCubeCount = 0;
            
            FindSameColoredNeighbours(tappedCube);

            if (chainedCubeCount < 2)
                return;

            GameEventSystem.Invoke<MoveMadeEvent>();
            
            var moreThanFive = chainedCubeCount > 6; // TODO: FIX THIS!! Must be > 4

            if (moreThanFive)
            {
                tappedCube.GetDestroyed();
                chainedItems.Remove(tappedCube);
                CreateRocket(tappedCube.Position);
            }

            foreach (var item in chainedItems)
            {
                item.GetDestroyed();
            }
            
            MakeItemsFall(chainedItems);
            SpawnNewItems(chainedItems);
        }
        
        private static void FindSameColoredNeighbours(Cube cube)
        {
            if (chainedItems.Contains(cube))
                return;
            
            chainedItems.Add(cube);
            
            var posX = cube.Position.x;
            var posY = cube.Position.y;

            const int n = MaxSize - 1;
            var adjacentItems = new Item[]
            {
                posX != 0 ? itemsOnBoard[posX - 1, posY] : null,
                posX != n ? itemsOnBoard[posX + 1, posY] : null,
                posY != 0 ? itemsOnBoard[posX, posY - 1] : null,
                posY != n ? itemsOnBoard[posX, posY + 1] : null
            };
            
            foreach (var item in adjacentItems)
            {
                if (item == null)
                    continue;
                
                item.AddToChain();
            }
        }

        public static void AddToChainedItems(Cube cube)
        {
            if (cube.Type != tappedCubeType)
                return;
            
            chainedCubeCount++;
            FindSameColoredNeighbours(cube);
        }

        public static void AddToChainedItems(Balloon balloon)
        {
            if (!chainedItems.Contains(balloon))
                chainedItems.Add(balloon);
        }

        private static void OnDuckHitBottom(object duck)
        {
            var hitDuck = (Duck)duck;
            
            RemoveItemFromBoard(hitDuck);
            ItemPooler.Return(hitDuck);

            var column = hitDuck.Position.y;
            
            MakeItemsFallAtColumn(column);
            SpawnNewItemsAtColumn(column, 1);
            
            GameEventSystem.Invoke<DuckDestroyedEvent>(duck);
        }

        private static void CreateRocket(Vector2Int gridPos)
        {
            var rocket = itemPooler.Get<Rocket>();
            var worldPos = gridManager.GetWorldPosition(gridPos);
            
            rocket.SetGridPositionAndSorting(gridPos);
            rocket.SetWorldPosition(worldPos);
            
            AddItemToBoard(rocket);
        }
        
        public static void OnRocketAction(Rocket rocket)
        {
            var origin = rocket.Position;

            var itemsToBeDestroyed = rocket.Type == RocketType.Horizontal
                ? itemsOnBoard.GetRow(origin.x)
                : itemsOnBoard.GetColumn(origin.y);
            
            foreach (var item in itemsToBeDestroyed)
            {
                if (!item)
                    continue;
                
                RemoveItemFromBoard(item);
                item.GetDestroyed();
            }
            
            if (rocket.Type == RocketType.Horizontal)
            {
                MakeItemsFall(itemsToBeDestroyed);
                SpawnNewItems(itemsToBeDestroyed);
            }
            else
            {
                SpawnNewItemsAtColumn(origin.y, count: boardSize.x, wholeColumn: true);
            }
        }
        
        private static void MakeItemsFall(IEnumerable<Item> destroyedItems)
        {
            destroyedItems = destroyedItems.Where(item => item != null);
            var columnIndices = destroyedItems.Select(item => item.Position.y).Distinct();

            foreach (var columnIndex in columnIndices)
            {
                MakeItemsFallAtColumn(columnIndex);
            }
        }

        private static void MakeItemsFallAtColumn(int columnIndex)
        {
            for (int j = Bottom + 1; j < Top + 1; j++)
            {
                var itemsInColumn = itemsOnBoard.GetColumn(columnIndex);
                    
                var item1 = itemsInColumn[j];

                if (!item1)
                    continue;

                for (int k = Bottom; k < j; k++)
                {
                    var item2 = itemsInColumn[k];

                    if (item2)
                        continue;

                    UpdateItemPos(item1, k, item1.Position.y);
                    var emptyPos = gridManager.GetWorldPosition(k, item1.Position.y);
                        
                    if (k == Bottom)
                        item1.MoveTo(emptyPos, isDuck: item1 is Duck);
                    else
                        item1.MoveTo(emptyPos);

                    break;
                }
            }
        }
        
        private static void SpawnNewItems(IEnumerable<Item> destroyedItems)
        {
            destroyedItems = destroyedItems.Where(item => item != null);
            var groups = destroyedItems.GroupBy(item => item.Position.y);
            
            foreach (var group in groups)
            {
                var column = group.Key;
                var count = group.Count();

                SpawnNewItemsAtColumn(column, count);
            }
        }

        private static void SpawnNewItemsAtColumn(int column, int count, bool wholeColumn = false)
        {
            for (int i =  Top - count + 1; i < Top + 1; i++)
            {
                var finalPos = new Vector2Int(i, column);
                    
                var randVal = Random.value;

                if (randVal < m_probabilities.Cube)
                {
                    var cube = (Cube)itemSpawner.Spawn<Cube>(finalPos, wholeColumn);
                    AddItemToBoard(cube);
                }
                else if (randVal < m_probabilities.Cube + m_probabilities.Balloon)
                {
                    var balloon = itemSpawner.Spawn<Balloon>(finalPos, wholeColumn);
                    AddItemToBoard(balloon);
                }
                else
                {
                    var duck = itemSpawner.Spawn<Duck>(finalPos, wholeColumn);
                    AddItemToBoard(duck);
                }
            }
        }
        
        private static void AddItemToBoard(Item item)
        {
            var pos = item.Position;
            itemsOnBoard[pos.x, pos.y] = item;
        }

        public static void RemoveItemFromBoard(Item item)
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
        
        private void ResizeBorders()
        {
            var size = m_borders.size;

            var correctionX = Mathf.Lerp(0, 0.08f, (9f - boardSize.y) / 9f);
            var correctionY = Mathf.Lerp(0, 0.33f, (9f - boardSize.x) / 9f);
            
            var newX = (boardSize.y / 9f) * size.x + correctionX;
            var newY = (boardSize.x / 9f) * size.y + correctionY;

            m_borders.size = new Vector2(newX, newY);
        }
        
        private static void FillItems()
        {
            itemsOnBoard = new Item[MaxSize, MaxSize];
            
            var range1 = Utilities.GetMidRange(boardSize.x);
            var range2 = Utilities.GetMidRange(boardSize.y);

            Bottom = range1.Start;
            Top = range1.End;
            
            for (int i = range1.Start; i < range1.End + 1; i++)
            {
                for (int j = range2.Start; j < range2.End + 1; j++)
                {
                    var cube = itemPooler.Get<Cube>();
                    
                    cube.SetGridPositionAndSorting(new Vector2Int(i, j));
                    cube.transform.position = gridManager.GetWorldPosition(i, j);

                    itemsOnBoard[i, j] = cube;
                }
            }
            
            GameEventSystem.Invoke<BoardLoadedEvent>();
        }
    }
}
