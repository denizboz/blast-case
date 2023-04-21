using UnityEngine;

namespace Board
{
    public enum RocketType { Horizontal, Vertical }
    
    public class Rocket : Booster
    {
        public RocketType Type;
        
        [SerializeField] private Transform m_leftOrUp;
        [SerializeField] private Transform m_rightOrDown;

        private const float halfOffset = 0.165f;
        public const int VarietySize = 2;
        
        public override void InitiateAction()
        {
            //
        }

        public void SetType(RocketType type)
        {
            Type = type;

            transform.rotation = type == RocketType.Horizontal ? Quaternion.identity : Quaternion.Euler(-90f * Vector3.back);
        }
    }
}
