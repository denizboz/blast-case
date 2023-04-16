using Board;
using UnityEngine;

namespace Utility
{
    public enum SpriteType { Balloon, Duck, Rocket }

    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "Sprite Container")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private Sprite m_duckSprite;
        [SerializeField] private Sprite m_rocketSprite;
        [SerializeField] private Sprite m_balloonSprite;
        
        [SerializeField] private Sprite[] m_cubeSprites;

        public Sprite GetSprite(SpriteType type)
        {
            if (type == SpriteType.Balloon)
                return m_balloonSprite;
            else if (type == SpriteType.Duck)
                return m_duckSprite;
            else
                return m_rocketSprite;
        }
        
        public Sprite GetCubeSprite(CubeType type)
        {
            return m_cubeSprites[(int)type];
        }

        /// <summary>
        /// Returns random CubeType and corresponding sprite as a tuple.
        /// </summary>
        public (CubeType, Sprite) GetRandomCube()
        {
            int rand = Random.Range(0, m_cubeSprites.Length);
            return ((CubeType)rand, m_cubeSprites[rand]);
        }
    }
}
