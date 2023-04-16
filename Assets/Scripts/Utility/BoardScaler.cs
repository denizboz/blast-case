using UnityEngine;

namespace Utility
{
    public class BoardScaler : MonoBehaviour
    {
        private const float refRatio = 16f / 9f;

        private void Awake()
        {
            ScaleBoard();
        }

        private void ScaleBoard()
        {
            var ratio = (float)Screen.width / Screen.height;
        
            var newScale = ratio / refRatio;
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
