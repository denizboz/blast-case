using UnityEngine;

namespace Utilities
{
    public class BoardScaler : MonoBehaviour
    {
        private void Awake()
        {
            ScaleBoard();
        }

        private void ScaleBoard()
        {
            var ratio = (float)Screen.width / Screen.height;
            const float refRatio = 9f / 16f;
            
            var newScale = ratio / refRatio;
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
