using Board;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "Sprite Container")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private Sprite m_duckSprite;
        [SerializeField] private Sprite m_rocketSprite;
        [SerializeField] private Sprite m_balloonSprite;
        
        [SerializeField] private Sprite[] m_cubeSprites;

        
        public Sprite GetSprite<T>(CubeType cubeType = CubeType.Yellow) where T : Item
        {
            var type = typeof(T);
            
            if (type == typeof(Cube))
                return m_cubeSprites[(int)cubeType];
            else if (type == typeof(Balloon))
                return m_balloonSprite;
            else if (type == typeof(Duck))
                return m_duckSprite;
            else
                return null;
                
        }
    }
}
