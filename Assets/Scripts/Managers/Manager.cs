using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Managers
{
    public abstract class Manager : MonoBehaviour
    {
        [SerializeField] protected DependencyContainerSO dependencyContainer;

        protected abstract void Awake();
    }
}
