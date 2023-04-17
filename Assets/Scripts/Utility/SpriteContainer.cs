using Board;
using UnityEngine;

namespace Utility
{
    public enum SpriteType { Cube, Balloon, Duck, Rocket }

    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "Sprite Container")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private Sprite m_duckSprite;
        [SerializeField] private Sprite m_rocketSprite;
        [SerializeField] private Sprite m_balloonSprite;
        
        [SerializeField] private Sprite[] m_cubeSprites;

        public Sprite GetSprite(SpriteType spriteType, CubeType cubeType = CubeType.Yellow)
        {
            if (spriteType == SpriteType.Cube)
                return m_cubeSprites[(int)cubeType];
            else if (spriteType == SpriteType.Balloon)
                return m_balloonSprite;
            else if (spriteType == SpriteType.Duck)
                return m_duckSprite;
            else
                return m_rocketSprite;
        }
    }
}
