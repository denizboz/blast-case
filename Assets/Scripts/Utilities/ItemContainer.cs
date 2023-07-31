using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ItemContainer", menuName = "Item Container")]
    public class ItemContainer : ContainerSO
    {
        public Item[] ItemPrefabs;
    }
}