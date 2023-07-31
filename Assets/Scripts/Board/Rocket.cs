using System;
using DG.Tweening;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Board
{
    public enum RocketType { Horizontal, Vertical }
    
    public class Rocket : Booster
    {
        public RocketType RocketType;
        
        [SerializeField] private Transform m_leftOrUp;
        [SerializeField] private Transform m_rightOrDown;

        private const float halfOffset = 0.165f;
        public static int VarietySize = Enum.GetValues(typeof(RocketType)).Length;
        
        public override void StartAnimation()
        {
            var leftFinalPos = m_leftOrUp.localPosition + 10f * Vector3.left;
            var rightFinalPos = m_rightOrDown.localPosition + 10f * Vector3.right;

            m_leftOrUp.DOLocalMove(leftFinalPos, 1f);
            m_rightOrDown.DOLocalMove(rightFinalPos, 1f);
        }

        public void SetType(RocketType type)
        {
            RocketType = type;
            transform.rotation = type == RocketType.Horizontal ? Quaternion.identity : Quaternion.Euler(-90f * Vector3.back);
            
            m_leftOrUp.localPosition = halfOffset * Vector3.left;
            m_rightOrDown.localPosition = halfOffset * Vector3.right;
        }

        public override void OnTap()
        {
            GameEventSystem.Invoke<MoveMadeEvent>();
            GameEventSystem.Invoke<RocketTappedEvent>(this);
        }
    }
}
