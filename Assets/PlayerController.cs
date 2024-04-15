using Components;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public float speed;
    public Camera playercamera;
    public GameObject gunPrefab;
    public GunController gun;
    public float suckCoinDistance;

    public Rigidbody playerrigidbody;

    public int coins = 0;

    public float invulnerableTimeLeft = 0;
    public float invulnerableTimeAfterHit;

    public Transform renderingContainer;

    public PlayerController()
    {
        Instance = this;
    }

    public void Start()
    {
        gun = Instantiate(gunPrefab, renderingContainer).GetComponent<GunController>();
        gun.holder = this;
    }

    private void FixedUpdate()
    {
        playerrigidbody.velocity *= 0.998f;
        //playerrigidbody.velocity *= 0.8f;
        if (invulnerableTimeLeft > 0)
            invulnerableTimeLeft -= Time.fixedDeltaTime;
    }

    void Update()
    {
        Vector3 force = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W))
        {
            force += speed * new Vector3(0,0,1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            force += -speed * new Vector3(1,0,0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            force += +speed * new Vector3(1,0,0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            force += -speed * new Vector3(0,0,1);
        }

        if (Input.GetKey(KeyCode.R))
        {
            foreach (var enemy in ArenaController.Instance.enemyContainer.GetComponentsInChildren<UnitController>())
            {
                enemy.Damage(1000f, Vector3.zero);
            }
        }
        Walk(force, 0.8f);

        //this.transform.position += Time.deltaTime * force;
        //playerrigidbody.AddForce(force);
        playercamera.transform.position = new Vector3(this.transform.position.x, this.playercamera.transform.position.y, this.transform.position.z - 17);
    }
    void Walk(Vector3 speed, float strength)
    {
        var dif = speed - playerrigidbody.velocity;
        //var speed_scale = (float)Vector3.Magnitude(speed) / (Vector3.Magnitude(dif) + Vector3.Magnitude(speed));
        //var speed_scale = 1.5f + (float)(Vector3.Dot(dif ,speed) / (Vector3.Magnitude(dif) * Vector3.Magnitude(speed) + 0.001));
        var traction = 1 / (Vector3.Magnitude(dif) + 1); // the larger the speed difference, the lower the traction
        //print("speed_scale " + traction);
        //speed_scale = speed_scale * speed_scale * speed_scale;
        playerrigidbody.velocity += traction  * strength * dif;
    }

    public void Damage(Vector3 knockback)
    {
        playerrigidbody.AddForce(knockback);
        if (invulnerableTimeLeft > 0)
        {
            return;
        }
        var upgrades = ArenaController.Instance.upgradeUi;
        upgrades.stats[UpgradeUIComponent.Health] -= 1;
        if (upgrades.stats[UpgradeUIComponent.Health] <= 0)
        {
            upgrades.stats[UpgradeUIComponent.Health] = 0;
            // todo, death sequence!
        }
        invulnerableTimeLeft = invulnerableTimeAfterHit;
        ArenaController.Instance.UpdateHud();
    }

    public int GetHealth()
    {
        return ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.Health];
    }
}
