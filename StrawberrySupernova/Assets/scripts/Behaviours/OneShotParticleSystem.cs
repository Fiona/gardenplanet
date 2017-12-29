using UnityEngine;

namespace StompyBlondie
{
    public class OneShotParticleSystem: MonoBehaviour
    {
        public ParticleSystem[] particles;

        private void Update()
        {
            foreach(var particle in particles)
                if(particle.IsAlive())
                    return;
            Destroy(gameObject);
        }
    }
}