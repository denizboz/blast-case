using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-50)]
    public class GameManager : Manager
    {
        protected override void Awake()
        {
            dependencyContainer.Bind<GameManager>(this);
            Application.targetFrameRate = 60;
        }
    }
}
