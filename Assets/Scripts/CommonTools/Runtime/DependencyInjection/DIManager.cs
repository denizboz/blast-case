using System.Linq;
using UnityEngine;

namespace CommonTools.Runtime.DependencyInjection
{
    public static class DIManager
    {
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BindDependencies()
        {
            var dependencies = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDependency>().ToArray();

            foreach (var dependency in dependencies)
            {
                // dependency.Bind();
            }
        }
    }
}