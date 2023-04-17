using UnityEngine;

namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item
    {
        public CubeType Type;

        public void SetType(CubeType type)
        {
            Type = type;
        }
    }
}
