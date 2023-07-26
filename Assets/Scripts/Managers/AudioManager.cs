using UnityEngine;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;

namespace Managers
{
    public class AudioManager : MonoBehaviour, IDependency
    {
    [SerializeField] private AudioSource m_source;

    [SerializeField] private AudioClip m_cubeExplodeSound;
    [SerializeField] private AudioClip m_balloonPopSound;
    [SerializeField] private AudioClip m_duckQuackSound;

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
