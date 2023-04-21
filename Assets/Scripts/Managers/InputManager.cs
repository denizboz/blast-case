using Board;
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
            
            GameEvents.AddListener(CoreEvent.GameWon, DisableInput);
            GameEvents.AddListener(CoreEvent.GameLost, DisableInput);
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
                if (hit.transform.TryGetComponent(out Item item))
                {
                    GameEvents.Invoke(BoardEvent.ItemTapped, item);
                }
            }
        }

        public void EnableInput()
        {
            m_isInputAllowed = true;
        }

        public void DisableInput()
        {
            m_isInputAllowed = false;
        }
    }
}
