using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem drawnParticles;
    [SerializeField] private ParticleSystem finishParticles;

    // Start is called before the first frame update
    void Start()
    {
        drawnParticles.gameObject.SetActive(false);
        finishParticles.gameObject.SetActive(false);
        finishParticles.Stop();
    }

    public void ActivateDrawParticles()
    {
        drawnParticles.gameObject.SetActive(true);
    }

    public void DeactivateDrawParticles()
    {
		drawnParticles.gameObject.SetActive(false);
	}
	public void StopDrawParticleEmission()
	{
		drawnParticles.Stop();
	}

	public void ActivateBurst()
    {
		finishParticles.gameObject.SetActive(true);
		finishParticles.Play();
    }
}
