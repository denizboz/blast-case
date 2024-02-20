using System.Linq;
using UnityEngine;

namespace CommonTools.Runtime.DependencyInjection
{
    [DefaultExecutionOrder(-500)]
    public class SceneBinder : MonoBehaviour
    {
        private void Awake()
        {
            BindDependencies();
        }

        private static void BindDependencies()
        {
            var dependencies = FindObjectsOfType<MonoBehaviour>().OfType<IDependency>().ToArray();

            foreach (var dependency in dependencies)
            {
                dependency.Bind();
            }
        }
    }
}