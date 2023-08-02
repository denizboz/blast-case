using System;
using DG.Tweening;
using Events;
using Events.Implementations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board
{
    public enum Orientation { Horizontal, Vertical }
    
    public class Rocket : Booster
    {
        public Orientation Orientation;
        
        [SerializeField] private Transform m_leftOrUp;
        [SerializeField] private Transform m_rightOrDown;

        private const float halfOffset = 0.165f;
        public static int VarietySize = Enum.GetValues(typeof(Orientation)).Length;
        
        public override void StartAnimation()
        {
            var leftFinalPos = m_leftOrUp.localPosition + 10f * Vector3.left;
            var rightFinalPos = m_rightOrDown.localPosition + 10f * Vector3.right;

            m_leftOrUp.DOLocalMove(leftFinalPos, 1f);
            m_rightOrDown.DOLocalMove(rightFinalPos, 1f);
        }

        public override void OrientRandom()
        {
            Orientation = Random.value > 0.5f ? Orientation.Horizontal : Orientation.Vertical;
            transform.rotation = Orientation == Orientation.Horizontal ? Quaternion.identity : Quaternion.Euler(-90f * Vector3.back);
            
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
