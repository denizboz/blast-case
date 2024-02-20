using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private Vector2 m_refAspectRatio = new Vector2(9f, 16f);
        
        private void Awake()
        {
            ScaleCamSize();
        }

        private void ScaleCamSize()
        {
            var attachedCam = GetComponent<Camera>();
            var currentRatio = (float)Screen.width / Screen.height;
            var refRatio = m_refAspectRatio.x / m_refAspectRatio.y;
            attachedCam.orthographicSize = attachedCam.orthographicSize * refRatio / currentRatio;
        }
    }
}