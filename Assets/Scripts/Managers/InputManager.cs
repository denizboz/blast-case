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

        public void EnableInput(bool val)
        {
            m_isInputAllowed = val;
        }
    }
}
