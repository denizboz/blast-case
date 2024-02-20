using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public static class EditorMenu
    {
        private const string initScenePath = "Assets/Scenes/InitScene.unity";
        private const string mainScenePath = "Assets/Scenes/MainScene.unity";

        
        [MenuItem("Peak/Open Init Scene")]
        private static void OpenInitScene() => OpenScene(initScenePath);
        
        [MenuItem("Peak/Open Main Scene")]
        private static void OpenMainScene() => OpenScene(mainScenePath);
        
        [MenuItem("Peak/Play from Splash")]
        private static void PlayFromSplash()
        {
            OpenInitScene();
            EditorApplication.EnterPlaymode();
        }

        private static void OpenScene(string path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path);
        }
    }
}