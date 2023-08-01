using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Utilities
{
    public class BoardScaler : MonoBehaviour, IDependency
    {
        public void Bind()
        {
            DI.Bind(this);
        }
        
        public void ScaleBoard()
        {
            var ratio = (float)Screen.width / Screen.height;
            const float refRatio = 9f / 16f;
            
            var newScale = ratio / refRatio;
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
