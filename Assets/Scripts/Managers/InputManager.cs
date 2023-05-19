using Board;
using Events;
using Events.Implementations;
using Events.Implementations.CoreEvents;
using Events.Implementations.InputEvents;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(-50)]
    public class InputManager : Manager
    {
        private static Camera mainCam;
        private static Ray ray;

        private bool m_isInputAllowed = true;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<InputManager>(this);
            Input.multiTouchEnabled = false;

            mainCam = Camera.main;
            
            GameEventSystem.AddListener<GameWonEvent>(DisableInput);
            GameEventSystem.AddListener<GameLostEvent>(DisableInput);
        }

        private void Update()
        {
            if (!m_isInputAllowed)
                return;
            
            if (!Input.GetMouseButtonDown(0))
                return;
            
            ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out ITappable item))
                {
                    GameEventSystem.Invoke<ItemTappedEvent>(item);
                }
            }
        }

        public void EnableInput(Object obj)
        {
            m_isInputAllowed = true;
        }

        private void DisableInput(object obj)
        {
            m_isInputAllowed = false;
        }
    }
}
