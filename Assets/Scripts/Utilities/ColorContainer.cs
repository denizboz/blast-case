using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ColorContainer", menuName = "New Color Container")]
    public class ColorContainer : ScriptableObject
    {
        [SerializeField] private Color[] m_cubeColors;

        public Color GetCubeColor(CubeType cubeType)
        {
            return m_cubeColors[(int)cubeType];
        }
    }
}