using Components;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public float speed;
    public Camera playercamera;
    public GameObject gunPrefab;
    public GameObject minionPrefab;
    public GunController gun;
    public float suckCoinDistance;

    public Rigidbody playerrigidbody;

    public int coins = 0;

    public float invulnerableTimeLeft = 0;
    public float invulnerableTimeAfterHit;

    public Transform renderingContainer;

    public PlayerState currentState = PlayerState.ALIVE;
    
    public float deathTicks = 0;

    public enum PlayerState
    {
        ALIVE,
        DEAD
    }

    public PlayerController()
    {
        Instance = this;
    }

    public void Start()
    {
        gun = Instantiate(gunPrefab, renderingContainer).GetComponent<GunController>();
        gun.Init(this.playerrigidbody, false, GunController.Guntype.Shotgun);
        ArenaController.Instance.upgradeUi.DoUpdateStats();
    }

    private void FixedUpdate()
    {
        playerrigidbody.velocity *= 0.998f;
        if (invulnerableTimeLeft > 0)
            invulnerableTimeLeft -= Time.fixedDeltaTime;
    }

    void Update()
    {
        switch (this.currentState)
        {
            case PlayerState.ALIVE:
            {
				//cheats
				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (Input.GetKeyDown(KeyCode.T))
					{
						gun.SetGuntype(GunController.Guntype.Rocketlauncher);
					}

					if (Input.GetKeyDown(KeyCode.Z))
					{
						gun.SetGuntype(GunController.Guntype.Shotgun);
					}

					if (Input.GetKeyDown(KeyCode.K))
					{
						coins += 10;
						ArenaController.Instance.UpdateHud();
					}

					if (Input.GetKeyDown(KeyCode.U))
					{
						ArenaController.Instance.SetStage(ArenaController.GameStage.UPGRADE);
					}

					if (Input.GetKeyDown(KeyCode.H))
					{
						var upgrades = ArenaController.Instance.upgradeUi;
						upgrades.stats[UpgradeUIComponent.Health] += 10;
					}

					if (Input.GetKeyDown(KeyCode.M))
					{
						MinionController minion = Instantiate(minionPrefab, ArenaController.Instance.friendContainer).GetComponent<MinionController>();
						minion.Init(3, playerrigidbody.position, 60);
					}

					if (Input.GetKeyDown(KeyCode.C))
					{
						gun.ChargeWithRockets(5);
					}

					if (Input.GetKey(KeyCode.R))
					{
						foreach (var enemy in ArenaController.Instance.enemyContainer.GetComponentsInChildren<UnitController>())
						{
							enemy.Damage(1000f, Vector3.zero);
						}
					}

				}
			    Vector3 direction = new Vector3(0, 0, 0);
                if (ArenaController.Instance.currentStage != ArenaController.GameStage.UPGRADE)
                {
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
				}
                direction = Vector3.Normalize(direction);
                Walk(direction * speed, 0.8f);
                break;
            }
            case PlayerState.DEAD:
            {
                Walk(Vector3.zero, 0.8f);
                GetComponent<Animator>().enabled = false;
                deathTicks += Time.deltaTime;
                if (deathTicks >= 3f)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                break;
            }
        }

        //this.transform.position += Time.deltaTime * force;
        //playerrigidbody.AddForce(force);
        if (currentState != PlayerState.DEAD)
        {
			playercamera.transform.position = new Vector3(this.transform.position.x, this.playercamera.transform.position.y, this.transform.position.z - 17);
		}
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
		if (currentState != PlayerState.ALIVE)
			return;

		if (AudioManager.Instance is not null)
            AudioManager.Instance.PlaySoundGotHit();
        
        playerrigidbody.AddForce(knockback);
        if (invulnerableTimeLeft > 0)
        {
            return;
        }
        var upgrades = ArenaController.Instance.upgradeUi;
        upgrades.stats[UpgradeUIComponent.Health] -= 1;
        ArenaController.Instance.UpdateHud();

		if (upgrades.stats[UpgradeUIComponent.Health] <= 0)
        {
            upgrades.stats[UpgradeUIComponent.Health] = 0;
            currentState = PlayerState.DEAD;

            // spawn broken cardboard
            CardboardDestroyer myRendererCardboardDestroyer = transform.GetComponentInChildren<CardboardDestroyer>();
            if (myRendererCardboardDestroyer != null)
            {
                myRendererCardboardDestroyer.SpawnDestroyedCardboard(20, knockback);
            }

            return;
        }
        invulnerableTimeLeft = invulnerableTimeAfterHit;
    }

    public int GetHealth()
    {
        return ArenaController.Instance.upgradeUi.stats[UpgradeUIComponent.Health];
    }
}
