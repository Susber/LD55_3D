using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update

    private float lifetime = 2;
    public Rigidbody bulletRigidbody;
    private bool enemyBullet =  true;
    private int n_to_hit = 1;
    void Start()
    {
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

    public void SetTeam(bool enemyBulletNew)
    {
        this.enemyBullet = enemyBulletNew;
        
    }
    
    void OnTriggerEnter(Collider coll)
    {
        if (n_to_hit < 1)
            return;
        var obstacle = coll.gameObject;
        
        if (enemyBullet)
        {
            var unit = obstacle.GetComponent<PlayerController>();
            if (unit != null)
            {
                unit.Damage();
                print("hit" + gameObject);
                Destroy(gameObject);
                n_to_hit -= 1;
            }
            
        }
        else
        {
            var unit = obstacle.GetComponent<UnitController>();
            if (unit != null)
            {
                unit.Damage(5);
 //               print("hit" + gameObject);
                Destroy(gameObject);
                n_to_hit -= 1;
            }
        }
    }
}
