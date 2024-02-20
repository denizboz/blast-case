using Board;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CommonTools.Runtime;

namespace UI
{
    public class GoalUI : MonoBehaviour
    {
        public Item Item;
        
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;
        [SerializeField] private TextMeshProUGUI m_tmp;

        public const float Width = 100f;
        
        public void Set(Item item, Sprite sprite)
        {
            Item = item;
            m_image.sprite = sprite;

            var y = m_rectTransform.sizeDelta.y;
            m_rectTransform.sizeDelta = m_rectTransform.sizeDelta.WithX(1f / 1.2f * y);

            gameObject.SetActive(true);
        }

        public void UpdateText(int val)
        {
            m_tmp.text = val.ToString();
        }
    }
}
