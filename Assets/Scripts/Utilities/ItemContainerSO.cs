using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ItemContainer", menuName = "Item Container")]
    public class ItemContainerSO : ContainerSO
    {
        public Item[] ItemPrefabs;
    }
}