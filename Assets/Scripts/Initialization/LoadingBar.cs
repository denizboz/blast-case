using CommonTools.Runtime;
using UnityEngine;

namespace Initialization
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField] private RectTransform m_fillBarRT;

        private Vector2 m_fillBarSizeDelta;
        private float m_fillBarFullWidth;

        private void Awake()
        {
            m_fillBarSizeDelta = m_fillBarRT.sizeDelta;
            m_fillBarFullWidth = m_fillBarSizeDelta.x;
        }

        public void SetFill(float value)
        {
            value = Mathf.Clamp01(value);
            m_fillBarRT.sizeDelta = m_fillBarSizeDelta.WithX(value * m_fillBarFullWidth);
        }
    }
}