using Components;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


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
        gun.Init(this.playerrigidbody);
        gun.SetGuntype(GunController.Guntype.Rocketlauncher);
        gun.guntype = GunController.Guntype.Rocketlauncher;
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
        Vector3 direction = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            direction += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += -new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += -new Vector3(0, 0, 1);
        }

        direction = Vector3.Normalize(direction);
        Walk(direction * speed, 0.8f);

        if (Input.GetKey(KeyCode.R))
        {
            foreach (var enemy in ArenaController.Instance.enemyContainer.GetComponentsInChildren<UnitController>())
            {
                enemy.Damage(1000f, Vector3.zero);
            }
        }

        //cheats
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.T))
            {
                gun.SetGuntype(GunController.Guntype.Rocketlauncher);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                gun.SetGuntype(GunController.Guntype.Shotgun);
            }

            if (Input.GetKey(KeyCode.H))
            {
                var upgrades = ArenaController.Instance.upgradeUi;
                upgrades.stats[UpgradeUIComponent.Health] += 100;
            }
        }

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
        playerrigidbody.velocity += traction * strength * dif;
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
            Thread.Sleep(5000);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
        invulnerableTimeLeft = invulnerableTimeAfterHit;
        ArenaController.Instance.UpdateHud();
    }

    public int GetHealth()
    {
        return ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.Health];
    }
}
