using UnityEngine;

namespace Utilities
{
    public abstract class ContainerSO : ScriptableObject
    {
        // using TryInitialize instead of static constructor as static constructors
        // of scriptable objects can be unintentionally called during editor time.
        public virtual void Initialize() { }
    }
}