using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Color = UnityEngine.Color;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update

    public float lifetime = 2;
    public Rigidbody bulletRigidbody;
    private bool shotFromEnemy =  true;
    private int n_to_hit = 1;
    private BulletType bullettype;
    public GameObject explosionPrefab;
    private ParticleSystem ps;
    private TrailRenderer tr;

    public enum BulletType
    {
        Bullet,
        Rocket
    };
    
    void Start()
    {
    }

    public void Init(BulletType type, Vector3 pos, Vector3 velocity, bool fromEnemy)
    {
        transform.position = pos;
        bulletRigidbody.position = pos;
        bulletRigidbody.velocity = velocity;
        SetTeam(fromEnemy);
        transform.rotation = Quaternion.LookRotation(velocity);
        ps = GetComponent<ParticleSystem>();
        tr = GetComponent<TrailRenderer>();
        setType(type);
    }

    public void setType(BulletType type)
    {
        this.bullettype = type;
        switch (type)
        {
            case BulletType.Bullet:
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case BulletType.Rocket:
                transform.localScale = new Vector3(2, 2, 2);
                ps.Play();
                tr.enabled = false;
                break;
        }
        if (type == BulletType.Rocket)
            lifetime = 10;
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

    public void SetTeam(bool fromEnemy)
    {
        this.shotFromEnemy = fromEnemy;
        
    }

    void OnHit()
    {
        n_to_hit -= 1;
        if (bullettype == BulletType.Rocket)
        {
            var explosion = Instantiate(explosionPrefab).GetComponent<ExplosionController>();
            explosion.Init(bulletRigidbody.position, 5, new Color(1f, 0.667f, 0f));
        }
        ps.Stop();
        Destroy(gameObject);
    }
    void OnTriggerEnter(Collider coll)
    {
        if (n_to_hit < 1)
            return;
        var obstacle = coll.gameObject;
        PlayerController player = obstacle.GetComponent<PlayerController>();
        MinionController minion = obstacle.GetComponent<MinionController>();
        bool collIsEnemy = !(player is not null || minion is not null);
        
        if(shotFromEnemy == collIsEnemy)
            return;
        
        var unit = obstacle.GetComponent<UnitController>();
        if (unit != null)
        {
            unit.Damage(PlayerController.Instance.gun.damage, bulletRigidbody.velocity.normalized * PlayerController.Instance.gun.knockback);
            OnHit();
        }
        else
        {
            if(player is not null){
                player.Damage(bulletRigidbody.velocity.normalized * PlayerController.Instance.gun.knockback);
                OnHit();
            }
        }
        
    }
}
