using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Rigidbody holder;
    public PlayerController playerController = null;
    
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public GameObject smoke;
    public Light myLight;

    public ParticleSystem ps;

    private float cooldown = 0.50f;
    public float knockback;
    private float light_timer = 1f;
    private float light_duration = 0.1f;
    
    private float max_recoil = 0.4f;
    private Vector3 lastShotDir = new Vector3(0, 0, 0); //direction where the gun is pushed

    private Vector3 playerGunPosition = new Vector3(0, 1, -0.5f); // default gun position
    public bool fromEnemy =  true;

    public int shootAmount;
    public float shootHalfAngle;  // in degrees

    public int level = 1;

    public static float rocketSpeed = 15;
    public static float bulletSpeed = 50;

    private float timeout = 0; //current cooldown, 0 means shooting is possible
    // Start is called before the first frame update

    private Guntype guntype;
    private int n_chargedrockets = 0;
    private bool ischarged = false;
    
    

    public enum Guntype
    {
        Rocketlauncher, Shotgun
    };
    
    void Start()
    {
        UpdateGunPosition();
        ps = GetComponentInChildren<ParticleSystem>();
        light_timer = light_duration + 1;
    }

    public void ChargeWithRockets(int n_rockets)
    {
        this.n_chargedrockets += n_rockets;
        ischarged = true;
        this.SetGuntype(Guntype.Rocketlauncher);
    }


    void shootRocket(Vector3 dir)
    {
        var bullet = Instantiate(bulletPrefab).GetComponent<BulletController>();
        int bonus = 0;
        if (ischarged)
            bonus = 2;
        bullet.Init(BulletController.BulletType.Rocket, transform.position, dir * rocketSpeed,2 + level + bonus, fromEnemy);
    }
    public Guntype GetGuntype()
    {
        return guntype;
    }
    public void SetGuntype(Guntype gunType2)
    {
        guntype = gunType2;
        UpdateGunStats();
    }


    public void SetLevel(int level2)
    {
        this.level = level2;
        UpdateGunStats();
    }

    private void UpdateGunStats()
    {
        switch (guntype)
        {
            case Guntype.Shotgun:
                cooldown = Mathf.Lerp(0.5f, 0.1f, (level - 1) / 5f);//2f / (1 + level);//
                break;
            case Guntype.Rocketlauncher:
                cooldown = 4f / (1 + level);
                break;
        }
    }
    
    void shootShotgun(Vector3 dir)
    {
        float shootHalfAngleInRad = shootHalfAngle / 360 * 2 * Mathf.PI;
        for (var i = 0; i < shootAmount; i++)
        {
            float shotAngle = shootHalfAngleInRad * ((float) ArenaController.Instance.rnd.NextDouble() * 2 - 1);
            var spreadDirection2d = Util.Rotate2d(new Vector2(dir.x, dir.z), shotAngle);
            var spreadDirection = new Vector3(spreadDirection2d.x, 0, spreadDirection2d.y);
            var bullet = Instantiate(bulletPrefab).GetComponent<BulletController>();
            bullet.Init(BulletController.BulletType.Bullet, transform.position, spreadDirection * bulletSpeed,level, this.fromEnemy );
            // bullet.SetTeam(false);
            // bullet.bulletRigidbody.velocity = spreadDirection * 50;
            // bullet.lifetime = bulletLifetime;
        }

    }
    public void TryShootAt(Vector3 worldpos)
    {
		if (ArenaController.Instance.currentStage == ArenaController.GameStage.UPGRADE)
		{
			return;
		}
		if (timeout <= 0){
            if (AudioManager.Instance is not null) { AudioManager.Instance.PlaySoundGun(); };
            myLight.enabled = true;
            light_timer = 0;
            var direction = worldpos - transform.position;
            direction.y = 0;
            direction = Vector3.Normalize(direction);
            this.smoke.transform.rotation = Quaternion.LookRotation(direction);
            lastShotDir = direction;
            //AudioManger.Instance.PlaySoundGun();
            this.ps.Play();
        
            switch (guntype)
            {
                case Guntype.Shotgun:
                    shootShotgun(direction);
                    break;
                case Guntype.Rocketlauncher:
                    shootRocket(direction);
                    break;
            }

            if (ischarged)
            {
                int n_chargedrockets_old = n_chargedrockets;
                n_chargedrockets -= 1;
                if (n_chargedrockets == 0 && n_chargedrockets_old == 1)
                {
                    SetGuntype(Guntype.Shotgun);
                    ischarged = false;
                }
            }

            timeout = cooldown;
        
        }
    }

    // Update is called once per frame
    void Update()
    {
        MaybeTurnGun();
        if (timeout <= 0) {
            bool left = Input.GetKey(KeyCode.Mouse0);
            bool right = Input.GetKey(KeyCode.Mouse1);
            if (playerController != null && (left || right))
            {
                float distance;
                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = playerController.playercamera.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 worldpos = ray.GetPoint(distance);
                    TryShootAt(worldpos);
                    if (right && Input.GetKey(KeyCode.LeftControl))
                    {
                        var explosion = Instantiate(explosionPrefab).GetComponent<ExplosionController>();
                        explosion.Init(worldpos, 10, Color.red);
                    }
                }
            }
        } else {
            timeout -= Time.deltaTime;
            UpdateGunPosition();
        }

        light_timer += Time.deltaTime;
        if (light_timer > light_duration)
        {
            myLight.enabled = false;
        }
    }

    private void MaybeTurnGun()
    {
        float distance;
        Plane plane = new Plane(Vector3.up, 0);
        Ray ray = PlayerController.Instance.playercamera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldpos = ray.GetPoint(distance);
            transform.localRotation = worldpos.x < transform.position.x ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void UpdateGunPosition()
    {
        var displacement = max_recoil * timeout / cooldown;
        transform.localPosition = playerGunPosition - displacement * lastShotDir;
        // recoil looks weird and changes depending on angle, reason is scale of player...
    }

    public void Init(Rigidbody rigidbody, bool fromEnemy2, Guntype guntype)
    {
        this.holder = rigidbody;
        var player = rigidbody.gameObject.GetComponent<PlayerController>();
        if (player is not null)
            this.playerController = player;
        else
            playerGunPosition = playerGunPosition + 0.2f * Vector3.up;
        this.fromEnemy = fromEnemy2;
        this.SetGuntype(guntype);

    }
}