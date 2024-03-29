using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Board;
using Board.Cubes;
using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Utilities;

namespace Managers
{
    public class BoardManager : MonoBehaviour, IDependency
    {
        [SerializeField] private ItemContainerSO m_itemContainer;

        private ItemTypeGenerator m_typeGenerator;
        
        private Item[,] m_itemsOnBoard;

        private List<Item> m_chainedItems;
        private int m_chainedCubeCount;

        private Type m_tappedCubeType;
        
        private ItemFactory m_itemFactory;
        private GridManager m_gridManager;
        private ItemSpawner m_itemSpawner;

        private Vector2Int m_boardSize;
        
        public static int Bottom;
        private static int top;

        public const int MinSize = 3;
        public const int MaxSize = 9;

        private const float spawnDelayStep = 0.15f;
        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_itemFactory = DI.Resolve<ItemFactory>();
            m_gridManager = DI.Resolve<GridManager>();

            m_itemSpawner = new ItemSpawner();
            
            GameEventSystem.AddListener<LevelLoadedEvent>(LoadItems);
            
            GameEventSystem.AddListener<CubeTappedEvent>(OnCubeTapped);
            GameEventSystem.AddListener<ItemDestroyedEvent>(RemoveItemFromBoard);
            GameEventSystem.AddListener<DuckHitBottomEvent>(OnDuckHitBottom);
            GameEventSystem.AddListener<RocketTappedEvent>(OnRocketTapped);
            
            GameEventSystem.AddListener<CubeLinkedToChainEvent>(FindSameColoredNeighbors);
            GameEventSystem.AddListener<BalloonAddedToChainEvent>(AddToChainedItems);
        }

        private void OnCubeTapped(object tappedCube)
        {
            var cube = (Cube)tappedCube;
            m_tappedCubeType = tappedCube.GetType();
            
            m_chainedItems = new List<Item>();
            m_chainedCubeCount = 0;
            
            FindSameColoredNeighbors(cube);

            if (m_chainedCubeCount < 2)
                return;

            GameEventSystem.Invoke<MoveMadeEvent>();
            
            var moreThanFive = m_chainedCubeCount > 4;

            if (moreThanFive)
            {
                cube.GetDestroyed();
                m_chainedItems.Remove(cube);
                CreateRocket(cube.Position);
            }

            foreach (var item in m_chainedItems)
            {
                item.GetDestroyed();
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
            var rocket = m_itemFactory.Get<Rocket>();
            rocket.OrientRandom();
            
            var worldPos = m_gridManager.GetWorldPosition(gridPos);
            
            rocket.SetGridPositionAndSorting(gridPos);
            rocket.SetWorldPosition(worldPos);
            
            AddItemToBoard(rocket);
        }
        
        private void OnRocketTapped(object tappedRocket)
        {
            var rocket = (Rocket)tappedRocket;
            var origin = rocket.Position;

            var itemsToBeDestroyed = rocket.Orientation == Orientation.Horizontal
                ? m_itemsOnBoard.GetRow(origin.x)
                : m_itemsOnBoard.GetColumn(origin.y);
            
            foreach (var item in itemsToBeDestroyed)
            {
                if (!item)
                    continue;
                
                RemoveItemFromBoard(item);
                item.GetDestroyed();
            }
            
            if (rocket.Orientation == Orientation.Horizontal)
            {
                MakeItemsFall(itemsToBeDestroyed);
                SpawnNewItems(itemsToBeDestroyed);
            }
            else
            {
                SpawnNewItemsAtColumn(origin.y, count: m_boardSize.x);
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
                        
                    item1.MoveTo(emptyPos);

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

        private void SpawnNewItemsAtColumn(int column, int count)
        {
            var iterator = 0;

            for (int i =  top - count + 1; i < top + 1; i++)
            {
                var itemType = m_typeGenerator.GetRandomItemType();
                var gridPos = new Vector2Int(i, column);

                var item = m_itemSpawner.Spawn(itemType, gridPos, spawnDelay: iterator * spawnDelayStep);
                AddItemToBoard(item);

                iterator++;
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
        
        private void LoadItems(object levelSO)
        {
            var level = (LevelSO)levelSO;
            var distribution = level.ProbDistribution;

            m_typeGenerator = new ItemTypeGenerator(m_itemContainer, distribution);

            m_boardSize = level.BoardSize;
            
            m_itemsOnBoard = new Item[MaxSize, MaxSize];

            var range1 = BoardUtility.GetMidRange(m_boardSize.x, MaxSize);
            var range2 = BoardUtility.GetMidRange(m_boardSize.y, MaxSize);

            Bottom = range1.Start;
            top = range1.End;

            for (int i = range1.Start; i < range1.End + 1; i++)
            {
                for (int j = range2.Start; j < range2.End + 1; j++)
                {
                    var type = m_typeGenerator.GetRandomCubeType();
                    var item = m_itemFactory.Get(type);

                    item.SetGridPositionAndSorting(new Vector2Int(i, j));
                    item.transform.position = m_gridManager.GetWorldPosition(i, j);

                    m_itemsOnBoard[i, j] = item;
                }
            }

            var scaler = DI.Resolve<BoardScaler>();
            scaler.ScaleBoard();
            
            GameEventSystem.Invoke<BoardLoadedEvent>(m_boardSize);
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelLoadedEvent>(LoadItems);
            
            GameEventSystem.RemoveListener<CubeTappedEvent>(OnCubeTapped);
            GameEventSystem.RemoveListener<ItemDestroyedEvent>(RemoveItemFromBoard);
            GameEventSystem.RemoveListener<DuckHitBottomEvent>(OnDuckHitBottom);
            GameEventSystem.RemoveListener<RocketTappedEvent>(OnRocketTapped);
            
            GameEventSystem.RemoveListener<CubeLinkedToChainEvent>(FindSameColoredNeighbors);
            GameEventSystem.RemoveListener<BalloonAddedToChainEvent>(AddToChainedItems);
        }
    }
}
