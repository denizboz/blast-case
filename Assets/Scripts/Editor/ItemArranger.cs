using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ItemArranger
    {
        private const int boardSize = 9;
        private const float boardWidth = 10.9f;
        private const float itemWidth = 1.19f;
        private const float interSpace = 0.1f;

        
        [MenuItem("Utilities/Arrange Selected Items")]
        private static void ArrangeItems()
        {
            var items = Selection.gameObjects.ToArray();

            const int count = boardSize * boardSize;

            if (items.Length == 1)
                items = items[0].GetComponentsInChildren<Transform>().Where(tr => tr != items[0].transform).Select(tr2 => tr2.gameObject).ToArray();
            
            if (items.Length != count)
                throw new Exception($"Number of selected items must be {count.ToString()} or 1 parent object with {count.ToString()} child objects.");

            for (int i = 0; i < count; i++)
            {
                int row = i / boardSize;
                int column = i % boardSize;

                var x = interSpace + (column + 0.5f) * itemWidth - boardWidth / 2f;
                var y = interSpace * 2f + (row + 0.5f) * itemWidth - boardWidth / 2f;

                var item = items[i];

                item.transform.localPosition = new Vector2(x, y);

                var sr = item.GetComponent<SpriteRenderer>();
                sr.size = new Vector2(itemWidth, itemWidth);
                sr.sortingOrder = row;
            }
        }
    }
}
