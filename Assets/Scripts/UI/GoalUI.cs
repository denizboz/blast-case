using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class GoalUI : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;
        [SerializeField] private TextMeshProUGUI m_tmp;

        private GoalType m_type;

        public void Set(GoalType type, Sprite sprite)
        {
            m_type = type;
            m_image.sprite = sprite;
            
            gameObject.SetActive(true);
        }

        public void SetPositionWithX(float x)
        {
            m_rectTransform.anchoredPosition = m_rectTransform.anchoredPosition.WithX(x);
        }
        
        public void UpdateText(int val)
        {
            m_tmp.text = val.ToString();
        }

        public float GetWidth()
        {
            return m_rectTransform.sizeDelta.x;
        }
    }
}
