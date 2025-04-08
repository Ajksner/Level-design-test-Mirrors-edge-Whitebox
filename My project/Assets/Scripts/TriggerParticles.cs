using UnityEngine;

public class TriggerParticles : MonoBehaviour
{
    [Header("Seznam Particle Systemů")]
    public ParticleSystem[] particles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (ParticleSystem ps in particles)
            {
                if (ps != null)
                    ps.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (ParticleSystem ps in particles)
            {
                if (ps != null)
                    ps.Stop();
            }
        }
    }
}
