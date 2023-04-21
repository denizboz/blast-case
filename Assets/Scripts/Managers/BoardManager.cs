using System.Collections.Generic;
using System.Linq;
using Board;
using Utility;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-10)]
    public class BoardManager : Manager
    {
        [SerializeField] private SpriteRenderer m_borders;
        
        private static Item[,] itemsOnBoard;
        
        private static List<Cube> sameColoredCubes;
        private static List<Balloon> poppedBalloons;
        private static IEnumerable<Item> destroyedItems;

        private static ItemPooler itemPooler;
        private static GridManager gridManager;
        private static ItemSpawner itemSpawner;
        private static InputManager inputManager;
        
        private static ProbabilityData m_probabilities;

        private static int bottom, top;

        public const int MinSize = 3;
        public const int MaxSize = 9;
        
        
        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
            
            GameEvents.AddListener(BoardEvent.ItemTapped, OnItemTap);
            GameEvents.AddListener(BoardEvent.DuckHitBottom, DestroyDuck);
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
                DestroyNeighbouringItems(cube);
            }
            else if (item is Rocket rocket)
            {
                rocket.InitiateAction();
            }
        }

        private void DestroyNeighbouringItems(Cube tappedCube)
        {
            sameColoredCubes = new List<Cube>();
            poppedBalloons = new List<Balloon>();
            
            FindSameColoredNeighbours(tappedCube);

            if (sameColoredCubes.Count < 2)
                return;

            bool moreThanFive = sameColoredCubes.Count > 4;
            
            if (sameColoredCubes.Count > 4)
            {
                sameColoredCubes.Remove(tappedCube);
                RemoveItemFromBoard(tappedCube);
                itemPooler.Return(tappedCube);
            }
            
            foreach (var cube in sameColoredCubes)
            {
                RemoveItemFromBoard(cube);
                itemPooler.Return(cube);
                
                GameEvents.Invoke(BoardEvent.CubeDestroyed, cube);
            }

            foreach (var balloon in poppedBalloons)
            {
                RemoveItemFromBoard(balloon);
                itemPooler.Return(balloon);
                
                GameEvents.Invoke(BoardEvent.BalloonPopped, balloon);
            }
            
            if (moreThanFive)
                CreateRocket(tappedCube.Position);
            
            MakeItemsFall();
            SpawnNewItems();
        }
        
        private static void FindSameColoredNeighbours(Cube centerCube)
        {
            if (sameColoredCubes.Contains(centerCube))
                return;
            
            sameColoredCubes.Add(centerCube);
            
            var posX = centerCube.Position.x;
            var posY = centerCube.Position.y;

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
                if (item is Balloon balloon)
                {
                    if (!poppedBalloons.Contains(balloon))
                        poppedBalloons.Add(balloon);

                    continue;
                }
                
                if (item is not Cube cube)
                    continue;
                
                if (cube.Type == centerCube.Type)
                    FindSameColoredNeighbours(cube);
            }
        }

        private void DestroyDuck(Item duck)
        {
            RemoveItemFromBoard(duck);
            itemPooler.Return(duck as Duck);

            var column = duck.Position.y;
            
            MakeItemsFallAtColumn(column);
            SpawnNewItemsAtColumn(column, 1);
        }

        private static void CreateRocket(Vector2Int gridPos)
        {
            var rocket = itemPooler.Get<Rocket>();
            var worldPos = gridManager.GetWorldPosition(gridPos);
            
            rocket.SetGridPositionAndSorting(gridPos);
            rocket.SetWorldPosition(worldPos);
            
            AddItemToBoard(rocket);
        }
        
        private static void MakeItemsFall()
        {
            destroyedItems = poppedBalloons.Count < 1
                ? sameColoredCubes
                : sameColoredCubes.Cast<Item>().Concat(poppedBalloons);
            
            var columnIndices = destroyedItems.Select(item => item.Position.y).Distinct();

            foreach (var columnIndex in columnIndices)
            {
                MakeItemsFallAtColumn(columnIndex);
            }
        }

        private static void MakeItemsFallAtColumn(int columnIndex)
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
                        
                    if (k == bottom)
                        item1.MoveTo(emptyPos, isDuck: item1 is Duck);
                    else
                        item1.MoveTo(emptyPos);

                    break;
                }
            }
        }
        
        private void SpawnNewItems()
        {
            var groups = destroyedItems.GroupBy(item => item.Position.y);
            
            foreach (var group in groups)
            {
                var column = group.Key;
                var count = group.Count();

                SpawnNewItemsAtColumn(column, count);
            }
        }

        private void SpawnNewItemsAtColumn(int column, int count)
        {
            for (int i =  top - count + 1; i < top + 1; i++)
            {
                var finalPos = new Vector2Int(i, column);
                    
                var randVal = Random.value;

                if (randVal < m_probabilities.Cube)
                {
                    var cube = (Cube)itemSpawner.Spawn<Cube>(finalPos);
                    AddItemToBoard(cube);
                }
                else if (randVal < m_probabilities.Cube + m_probabilities.Balloon)
                {
                    var balloon = itemSpawner.Spawn<Balloon>(finalPos);
                    AddItemToBoard(balloon);
                }
                else
                {
                    var duck = itemSpawner.Spawn<Duck>(finalPos);
                    AddItemToBoard(duck);
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
        
        private static void FillItems(Vector2Int boardSize)
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
                    
                    cube.SetGridPositionAndSorting(new Vector2Int(i, j));
                    cube.transform.position = gridManager.GetWorldPosition(i, j);

                    itemsOnBoard[i, j] = cube;
                }
            }
        }
    }
}
