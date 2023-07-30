using System.Collections.Generic;
using Board;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class ParticlesManager : MonoBehaviour, IDependency
    {
        [SerializeField] private GameObject m_particlesPrefab;
        [SerializeField] private ColorContainer m_colorContainer;

        private Queue<ParticleSystem> m_particlesPool;
        private const int PoolSize = 64;
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            CreatePool();
            GameEventSystem.AddListener<CubePoppedEvent>(Play);
        }

        private void Play(object poppedCube)
        {
            var cube = (Cube)poppedCube;
            var type = cube.Type;

            var particles = m_particlesPool.Dequeue();
            particles.transform.position = cube.WorldPosition;
    
            var mainModule = particles.main;
            mainModule.startColor = m_colorContainer.GetCubeColor(type);
            
            particles.Play();
            m_particlesPool.Enqueue(particles);
        }
        
        private void CreatePool()
        {
            m_particlesPool = new Queue<ParticleSystem>(PoolSize);

            for (int i = 0; i < PoolSize; i++)
            {
                var particlesObject = Instantiate(m_particlesPrefab, transform);
                var particles = particlesObject.GetComponent<ParticleSystem>();
                
                m_particlesPool.Enqueue(particles);
            }
        }
    }
}