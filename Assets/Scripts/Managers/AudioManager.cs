using UnityEngine;
using Board;
using Events;
using Events.Implementations.Board;

namespace Managers
{
    public class AudioManager : Manager
    {
        [SerializeField] private AudioSource m_source;
        
        [SerializeField] private AudioClip m_cubeExplodeSound;
        [SerializeField] private AudioClip m_balloonPopSound;
        [SerializeField] private AudioClip m_duckQuackSound;
        
        protected override void Awake()
        {
            dependencyContainer.Bind<AudioManager>(this);
            
            GameEventSystem.AddListener<CubeDestroyedEvent>(PlayCubeExplosion);
            GameEventSystem.AddListener<BalloonDestroyedEvent>(PlayBalloonPop);
            GameEventSystem.AddListener<DuckHitBottomEvent>(PlayDuckQuack);
        }

        private void PlayCubeExplosion(object obj)
        {
            m_source.clip = m_cubeExplodeSound;
            m_source.Play();
        }

        private void PlayBalloonPop(object obj)
        {
            m_source.clip = m_balloonPopSound;
            m_source.Play();
        }
        
        private void PlayDuckQuack(object obj)
        {
            m_source.clip = m_duckQuackSound;
            m_source.Play();
        }
    }
}
