using UnityEngine;

namespace Utilities
{
    public static class ContainerInit
    {
        private const string containersDir = "Containers";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var containers = Resources.LoadAll<ContainerSO>(containersDir);

            foreach (var container in containers)
            {
                container.Initialize();
            }
        }
    }
}