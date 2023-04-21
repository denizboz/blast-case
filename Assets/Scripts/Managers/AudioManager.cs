using UnityEngine;
using Board;

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
            
            GameEvents.AddListener(BoardEvent.CubeDestroyed, PlayCubeExplosion);
            GameEvents.AddListener(BoardEvent.BalloonPopped, PlayBalloonPop);
            GameEvents.AddListener(BoardEvent.DuckHitBottom, PlayDuckQuack);
        }

        private void PlayCubeExplosion(Item item)
        {
            m_source.clip = m_cubeExplodeSound;
            m_source.Play();
        }

        private void PlayBalloonPop(Item item)
        {
            m_source.clip = m_balloonPopSound;
            m_source.Play();
        }
        
        private void PlayDuckQuack(Item item)
        {
            m_source.clip = m_duckQuackSound;
            m_source.Play();
        }
    }
}
