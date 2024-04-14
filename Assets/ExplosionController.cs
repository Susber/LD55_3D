using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    void Start(float size2)
    {
        this.spherecollider.radius = size2;
        this.size = size2;
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
            if (timer <= 0)
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
        var force_magnitude = (size - distance) / size;
        var force = dir * force_magnitude * size * 2;
        rigidbody.AddForce(force);

    }
    private void explode()
    {
        exploded = true;
        particlesystem.Play();
        for(int I =0; I < AffectedObjects.Count; I++)
        {
            var rigidbody = AffectedObjects[I];
            if(rigidbody is not null)
                PushRigidbody(rigidbody);
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
