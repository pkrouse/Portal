using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private PortalController OtherPortal;
    [SerializeField] ParticleSystem particles;
    void Start()
    {
       
    }

    public void ShutDownParticles()
    {
        particles.Stop();
    }
}
