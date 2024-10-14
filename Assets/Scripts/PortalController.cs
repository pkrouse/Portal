using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private PortalController OtherPortal;
    [SerializeField] ParticleSystem[] particles = new ParticleSystem[2];
    void Start()
    {
       
    }

    public void ShutDownParticles()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
    }
}
