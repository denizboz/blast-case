using System;
using System.Linq;
using Board;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "ProbabilityDistribution", menuName = "Probability Distribution")]
    public class ProbabilityDistributionSO : ScriptableObject
    {
        public OccurrenceInfo<Item>[] Occurrences;

        // debug purpose only. can be made readonly with an inspector tool such as Odin.
        [SerializeField] private int m_totalCount;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_totalCount = Occurrences.Sum(info => info.Count);
        }
#endif
    }

    [Serializable]
    public struct OccurrenceInfo<T>
    {
        public T Item;
        public int Count;
    }
}