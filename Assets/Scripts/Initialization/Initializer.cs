using System.Collections;
using System.Linq;
using CommonTools.Runtime.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private LoadingBar m_loadingBar;
        
        private void Start()
        {
            StartCoroutine(nameof(LoadMainSceneAsync));
        }

        private IEnumerator LoadMainSceneAsync()
        {
            var mainSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            var asyncLoad = SceneManager.LoadSceneAsync(mainSceneIndex, LoadSceneMode.Single);

            m_loadingBar.SetFill(0f);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.isDone)
            {
                m_loadingBar.SetFill(asyncLoad.progress);
                yield return null;
            }
            
            asyncLoad.allowSceneActivation = true;
        }

        private static void BindDependencies()
        {
            var dependencies = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDependency>().ToArray();

            foreach (var dependency in dependencies)
            {
                dependency.Bind();
            }
        }
    }
}