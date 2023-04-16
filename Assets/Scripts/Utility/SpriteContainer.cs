using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "New Sprite Container")]
    public class SpriteContainer : ScriptableObject
    {
        public Sprite DuckSprite;
        public Sprite RocketSprite;
        public Sprite BalloonSprite;
        
        public Sprite[] CubeSprites;
    }
}
