using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Components;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float timer = 3;
    public bool exploded = false;
    public float lifetime = 5;
    public float size = 5;
    public List<Rigidbody> AffectedObjects;
    public Vector3 ForceVector;
    public ParticleSystem particlesystem;

    public SphereCollider spherecollider;
    
    public void Init(Vector3 pos, float size2, Color c)
    {
        spherecollider = GetComponent<SphereCollider>();
        particlesystem = GetComponent<ParticleSystem>();
        particlesystem.Stop();
        var main = particlesystem.main;
        main.startColor = new ParticleSystem.MinMaxGradient(c, Color.white);
        this.spherecollider.radius = size2;
        this.size = size2;
        transform.position = pos;
        timer = 0.1f;
    }

    void Update()
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
        var force_magnitude = (size - distance) / size; // one at center, 0 at border of explosion (distance = size)
        var force = dir * force_magnitude * size * 400;
        rigidbody.AddForce(force);
        
        PlayerController playerController = rigidbody.gameObject.GetComponent<PlayerController>();
                if (playerController is not null)
                    playerController.Damage();
        UnitController unitController = rigidbody.gameObject.GetComponent<UnitController>();
        if (unitController is not null)
            unitController.Damage(force_magnitude * size * 10);
    }
    private void explode()
    {
        exploded = true;
        particlesystem.Play();
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
