using Board;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using CommonTools.Runtime;

namespace UI
{
    public class GoalUI : MonoBehaviour
    {
        public GoalType Type;
        
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;
        [SerializeField] private TextMeshProUGUI m_tmp;

        public const float Width = 100f;
        
        public void Set(GoalType type, Sprite sprite)
        {
            Type = type;
            m_image.sprite = sprite;

            if ((int)type < Cube.VarietySize)
            {
                var y = m_rectTransform.sizeDelta.y;
                m_rectTransform.sizeDelta = m_rectTransform.sizeDelta.WithX(1f / 1.2f * y);
            }

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
