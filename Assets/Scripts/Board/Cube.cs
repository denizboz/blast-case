namespace Board
{
    public enum CubeType { Yellow, Red, Blue, Green, Purple }
    
    public class Cube : Item
    {
        public CubeType Type;

        public const int VarietySize = 5;

        public void SetType(CubeType type)
        {
            Type = type;
        }
    }
}
