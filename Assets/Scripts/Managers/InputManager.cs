using Board;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Managers
{
    public class InputManager : MonoBehaviour, IDependency
    {
        private Camera m_mainCam;
        private Ray m_ray;

        private bool m_isInputAllowed = true;
        
        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            Input.multiTouchEnabled = false;
            m_mainCam = Camera.main;
            
            GameEventSystem.AddListener<GameWonEvent>(DisableInput);
            GameEventSystem.AddListener<GameLostEvent>(DisableInput);
        }

        private void Update()
        {
            if (!m_isInputAllowed)
                return;
            
            if (!Input.GetMouseButtonDown(0))
                return;
            
            m_ray = m_mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(m_ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Item item))
                {
                    item.OnTap();
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
