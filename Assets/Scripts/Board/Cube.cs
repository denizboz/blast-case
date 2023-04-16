using UnityEngine;

namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item
    {
        public CubeType Type;

        public void SetType(CubeType type, Sprite sprite)
        {
            Type = type;
            SRenderer.sprite = sprite;
        }

        public void SetPositionAndSorting(int x, int y)
        {
            Position = new Vector2Int(x, y);
            SRenderer.sortingOrder = y;
        }
    }
}
