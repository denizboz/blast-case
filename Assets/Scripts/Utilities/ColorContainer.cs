using System;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ColorContainer", menuName = "Color Container")]
    public class ColorContainer : ContainerSO
    {
        [SerializeField] private ItemProperty<Item, Color>[] m_itemColors;

        private Dictionary<Type, Color> m_colorDictionary;

        
        public override void Initialize()
        {
            m_colorDictionary = new Dictionary<Type, Color>(m_itemColors.Length);
            
            foreach (var pair in m_itemColors)
            {
                m_colorDictionary.Add(pair.Item.GetType(), pair.Property);
            }
        }
        
        public Color GetColor(Item item)
        {
            var type = item.GetType();
            return m_colorDictionary[type];
        }
    }
}