using UnityEngine;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;

namespace Managers
{
    public class AudioManager : MonoBehaviour, IDependency
    {
        [SerializeField] private AudioSource m_cubeAudioSource;
        [SerializeField] private AudioSource m_balloonAudioSource;
        [SerializeField] private AudioSource m_duckAudioSource;
        
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            GameEventSystem.AddListener<CubePoppedEvent>(PlayCubeExplosion);
            GameEventSystem.AddListener<BalloonPoppedEvent>(PlayBalloonPop);
            GameEventSystem.AddListener<DuckHitBottomEvent>(PlayDuckQuack);
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<CubePoppedEvent>(PlayCubeExplosion);
            GameEventSystem.RemoveListener<BalloonPoppedEvent>(PlayBalloonPop);
            GameEventSystem.RemoveListener<DuckHitBottomEvent>(PlayDuckQuack);
        }

        private void PlayCubeExplosion(object obj)
        {
            m_cubeAudioSource.Play();
        }

        private void PlayBalloonPop(object obj)
        {
            m_balloonAudioSource.Play();
        }

        private void PlayDuckQuack(object obj)
        {
            m_duckAudioSource.Play();
        }
    }
}
