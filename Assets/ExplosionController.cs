using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float timer = 3;
    public bool exploded = false;
    public float lifetime = 5;
    public float size = 5;
    public List<Rigidbody> AffectedObjects;
    public Vector3 ForceVector;
    public ParticleSystem sparkparticles;
    public ParticleSystem smokeparticles;

    public SphereCollider spherecollider;
    private float explosion_range_nerf = 2.0f;
    
    public void Init(Vector3 pos, float size2, Color c)
    {
        spherecollider = GetComponent<SphereCollider>();
        sparkparticles = GetComponent<ParticleSystem>();
        smokeparticles = GetComponentInChildren<ParticleSystem>();
        sparkparticles.Stop();
        smokeparticles.Stop();
        
        
        
        this.spherecollider.radius = size2-explosion_range_nerf;
        this.size = size2; //size scales the explosion size, default is which is represented by the default explosion animation
        transform.position = pos;
        timer = 0.1f;
        
        var sparkles_main = sparkparticles.main;
        sparkles_main.startColor = new ParticleSystem.MinMaxGradient(c, Color.yellow); 
        scaleParticleSpeed(sparkparticles, size / 5);
        scaleParticleBurstCount(sparkparticles, size / 5);
        
        var smoke_main = smokeparticles.colorOverLifetime;
        smoke_main.color = new ParticleSystem.MinMaxGradient(c, new Color(0.8f,0.8f, 0.8f));
        scaleParticleSpeed(smokeparticles, Mathf.Sqrt(size / 5));
        scaleParticleBurstCount(smokeparticles, Mathf.Sqrt(size / 5));
    }

    void scaleParticleBurstCount(ParticleSystem ps, float scale)
    {
        var burst = sparkparticles.emission.GetBurst(0);
        burst.count = new ParticleSystem.MinMaxCurve((float) burst.count.constantMin * scale);
    }
    void scaleParticleSpeed(ParticleSystem ps, float scale)
    {
        var main = sparkparticles.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constantMin * scale, main.startSpeed.constantMax * scale);
    }

    void FixedUpdate()
    {
        if(!exploded)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                explode();
            }
        }
        else
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                Destroy(this);
            }
        }
    }

    private void PushRigidbody(Rigidbody rigidbody)
    {
        var displacement = rigidbody.position - this.transform.position;
        var dir = Vector3.Normalize(displacement);
        var distance = Vector3.Magnitude(displacement);
        var force_magnitude = ((size-explosion_range_nerf) - distance) / (size-explosion_range_nerf); // one at center, 0 at border of explosion (distance = size)
        var knockback = force_magnitude * (size-explosion_range_nerf) * 150 * dir;
        
        PlayerController playerController = rigidbody.gameObject.GetComponent<PlayerController>();
        if (playerController is not null)
            playerController.playerrigidbody.AddForce(knockback/1.5f);
        UnitController unitController = rigidbody.gameObject.GetComponent<UnitController>();
        if (unitController is not null)
            unitController.Damage(force_magnitude * size, knockback);
    }
    private void explode()
    {
        if (AudioManager.Instance is not null)
            AudioManager.Instance.PlaySoundExplosion();
        exploded = true;
        sparkparticles.Play();
        smokeparticles.Play();
        for(int I =0; I < AffectedObjects.Count; I++)
        {
            var affectedrigidbody = AffectedObjects[I];
            if(!affectedrigidbody.IsDestroyed())
                PushRigidbody(affectedrigidbody);
        }

        this.GetComponent<SphereCollider>().enabled = false;

    }
    
 
    void OnTriggerEnter(Collider collidee)
    {
        AffectedObjects.Add(collidee.GetComponent<Rigidbody>());
    }
 
    void OnTriggerExit(Collider collidee)
    {
        AffectedObjects.Remove(collidee.GetComponent<Rigidbody>());
    }
    
}
