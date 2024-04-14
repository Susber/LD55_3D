using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update

    public float lifetime = 0;
    public Rigidbody bulletRigidbody;
    void Start()
    {
        lifetime = 3;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        var incomingRigidbody = coll.rigidbody;
        var obstracle = coll.rigidbody.gameObject.GetComponent<UnitController>();
        print("hit" + obstracle);
        obstracle.Damage(5);
        Destroy(this.gameObject);
    }
}
