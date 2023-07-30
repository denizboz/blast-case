using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Board;
using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BoardManager : MonoBehaviour, IDependency
    {
        [SerializeField] private SpriteRenderer m_borders;
        
        private Item[,] m_itemsOnBoard;

        private List<Item> m_chainedItems;
        private int m_chainedCubeCount;

        private CubeType m_tappedCubeType;
        
        private ItemPooler m_itemPooler;
        private GridManager m_gridManager;
        private ItemSpawner m_itemSpawner;

        private Vector2Int m_boardSize;
        
        private ProbabilityData m_probabilities;

        public static int Bottom;
        private static int top;

        public const int MinSize = 3;
        public const int MaxSize = 9;
        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_itemPooler = DI.Resolve<ItemPooler>();
            m_gridManager = DI.Resolve<GridManager>();
            m_itemSpawner = DI.Resolve<ItemSpawner>();
            
            GameEventSystem.AddListener<CubeTappedEvent>(OnCubeTapped);
            GameEventSystem.AddListener<ItemDestroyedEvent>(RemoveItemFromBoard);
            GameEventSystem.AddListener<DuckHitBottomEvent>(OnDuckHitBottom);
            GameEventSystem.AddListener<RocketTappedEvent>(OnRocketTapped);
            
            GameEventSystem.AddListener<CubeLinkedToChainEvent>(FindSameColoredNeighbors);
            GameEventSystem.AddListener<BalloonAddedToChainEvent>(AddToChainedItems);
        }

        private void Start()
        {
            var gameManager = DI.Resolve<GameManager>();
            m_boardSize = gameManager.GetCurrentBoardSize();
            
            FillItems();
            ResizeBorders();
            
            SetFallProbabilities();
        }

        private void OnCubeTapped(object tappedCube)
        {
            var cube = (Cube)tappedCube;

            m_tappedCubeType = cube.Type;
            
            m_chainedItems = new List<Item>();
            m_chainedCubeCount = 0;
            
            FindSameColoredNeighbors(cube);

            if (m_chainedCubeCount < 2)
                return;

            GameEventSystem.Invoke<MoveMadeEvent>();
            
            var moreThanFive = m_chainedCubeCount > 4;

            if (moreThanFive)
            {
                cube.GetDestroyed(); // HERE
                m_chainedItems.Remove(cube);
                CreateRocket(cube.Position);
            }

            foreach (var item in m_chainedItems)
            {
                item.GetDestroyed(); // HERE
            }
            
            MakeItemsFall(m_chainedItems);
            SpawnNewItems(m_chainedItems);
        }
        
        private void FindSameColoredNeighbors(object linkedCube)
        {
            var cube = (Cube)linkedCube;
            
            if (m_chainedItems.Contains(cube))
                return;
            
            m_chainedItems.Add(cube);
            m_chainedCubeCount++;
            
            var posX = cube.Position.x;
            var posY = cube.Position.y;

            const int n = MaxSize - 1;
            var adjacentItems = new Item[]
            {
                posX != 0 ? m_itemsOnBoard[posX - 1, posY] : null,
                posX != n ? m_itemsOnBoard[posX + 1, posY] : null,
                posY != 0 ? m_itemsOnBoard[posX, posY - 1] : null,
                posY != n ? m_itemsOnBoard[posX, posY + 1] : null
            };
            
            foreach (var item in adjacentItems)
            {
                if (item == null)
                    continue;
                
                if (item.IsChainable(m_tappedCubeType))
                    item.AddToItemChain();
            }
        }

        private void AddToChainedItems(object addedItem)
        {
            var item = (Item)addedItem;

            if (m_chainedItems.Contains(item))
                return;
            
            m_chainedItems.Add(item);
        }
        
        private void OnDuckHitBottom(object duck)
        {
            var hitDuck = (Duck)duck;

            hitDuck.GetDestroyed();
            
            var column = hitDuck.Position.y;
            
            MakeItemsFallAtColumn(column);
            SpawnNewItemsAtColumn(column, 1);
        }

        private void CreateRocket(Vector2Int gridPos)
        {
            var rocket = m_itemPooler.Get<Rocket>();
            var worldPos = m_gridManager.GetWorldPosition(gridPos);
            
            rocket.SetGridPositionAndSorting(gridPos);
            rocket.SetWorldPosition(worldPos);
            
            AddItemToBoard(rocket);
        }
        
        private void OnRocketTapped(object tappedRocket)
        {
            var rocket = (Rocket)tappedRocket;
            var origin = rocket.Position;

            var itemsToBeDestroyed = rocket.Type == RocketType.Horizontal
                ? m_itemsOnBoard.GetRow(origin.x)
                : m_itemsOnBoard.GetColumn(origin.y);
            
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
                SpawnNewItemsAtColumn(origin.y, count: m_boardSize.x, wholeColumn: true);
            }
        }
        
        private void MakeItemsFall(IEnumerable<Item> destroyedItems)
        {
            destroyedItems = destroyedItems.Where(item => item != null);
            var columnIndices = destroyedItems.Select(item => item.Position.y).Distinct();

            foreach (var columnIndex in columnIndices)
            {
                MakeItemsFallAtColumn(columnIndex);
            }
        }

        private void MakeItemsFallAtColumn(int columnIndex)
        {
            for (int j = Bottom + 1; j < top + 1; j++)
            {
                var itemsInColumn = m_itemsOnBoard.GetColumn(columnIndex);
                    
                var item1 = itemsInColumn[j];

                if (!item1)
                    continue;

                for (int k = Bottom; k < j; k++)
                {
                    var item2 = itemsInColumn[k];

                    if (item2)
                        continue;

                    UpdateItemPos(item1, k, item1.Position.y);
                    var emptyPos = m_gridManager.GetWorldPosition(k, item1.Position.y);
                        
                    item1.MoveTo(emptyPos, toBottom: item1.Position.x == Bottom);

                    break;
                }
            }
        }
        
        private void SpawnNewItems(IEnumerable<Item> destroyedItems)
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

        private void SpawnNewItemsAtColumn(int column, int count, bool wholeColumn = false)
        {
            for (int i =  top - count + 1; i < top + 1; i++)
            {
                var finalPos = new Vector2Int(i, column);
                    
                var randVal = Random.value;

                if (randVal < m_probabilities.Cube)
                {
                    var cube = (Cube)m_itemSpawner.Spawn<Cube>(finalPos, wholeColumn);
                    AddItemToBoard(cube);
                }
                else if (randVal < m_probabilities.Cube + m_probabilities.Balloon)
                {
                    var balloon = m_itemSpawner.Spawn<Balloon>(finalPos, wholeColumn);
                    AddItemToBoard(balloon);
                }
                else
                {
                    var duck = m_itemSpawner.Spawn<Duck>(finalPos, wholeColumn);
                    AddItemToBoard(duck);
                }
            }
        }
        
        private void AddItemToBoard(Item item)
        {
            var pos = item.Position;
            m_itemsOnBoard[pos.x, pos.y] = item;
        }

        private void RemoveItemFromBoard(object item)
        {
            var pos = ((Item)item).Position;
            m_itemsOnBoard[pos.x, pos.y] = null;
        }

        private void UpdateItemPos(Item item, int x, int y)
        {
            var oldPos = item.Position;
            m_itemsOnBoard[oldPos.x, oldPos.y] = null;
            
            item.SetGridPositionAndSorting(new Vector2Int(x, y));
            m_itemsOnBoard[x, y] = item;
        }
        
        private void SetFallProbabilities()
        {
            var gameManager = DI.Resolve<GameManager>();
            m_probabilities = gameManager.GetItemProbabilities();
        }
        
        private void ResizeBorders()
        {
            var size = m_borders.size;

            var correctionX = Mathf.Lerp(0, 0.08f, (9f - m_boardSize.y) / 9f);
            var correctionY = Mathf.Lerp(0, 0.33f, (9f - m_boardSize.x) / 9f);
            
            var newX = (m_boardSize.y / 9f) * size.x + correctionX;
            var newY = (m_boardSize.x / 9f) * size.y + correctionY;

            m_borders.size = new Vector2(newX, newY);
        }
        
        private void FillItems()
        {
            m_itemsOnBoard = new Item[MaxSize, MaxSize];

            var range1 = BoardUtility.GetMidRange(m_boardSize.x, MaxSize);
            var range2 = BoardUtility.GetMidRange(m_boardSize.y, MaxSize);

            Bottom = range1.Start;
            top = range1.End;
            
            for (int i = range1.Start; i < range1.End + 1; i++)
            {
                for (int j = range2.Start; j < range2.End + 1; j++)
                {
                    var cube = m_itemPooler.Get<Cube>();
                    
                    cube.SetGridPositionAndSorting(new Vector2Int(i, j));
                    cube.transform.position = m_gridManager.GetWorldPosition(i, j);

                    m_itemsOnBoard[i, j] = cube;
                }
            }
            
            GameEventSystem.Invoke<BoardLoadedEvent>();
        }
    }
}
