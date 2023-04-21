using UnityEngine;

namespace Board
{
    public enum RocketType { Horizontal, Vertical }
    
    public class Rocket : Booster
    {
        [SerializeField] private RocketType m_type;
        
        [SerializeField] private Transform m_leftOrUp;
        [SerializeField] private Transform m_rightOrDown;

        private const float halfOffset = 0.165f;
        
        public override void InitiateAction()
        {
            //
        }

        public void ResetSides()
        {
            
        }
    }
}
