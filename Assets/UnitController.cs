using UnityEngine;

namespace Components
{
    public class UnitController : MonoBehaviour
    {
        public Rigidbody unitRigidbody;
        public Transform turntransform;
        public bool invertDirection;

        public float life = 100;
        private float armor_factor = 1;
        private float armor;

        public int coins;

        public float pushKnockback;

        public float currentangle = 0;
        public float targetangle = 0;
        public float rotationmomentum = 0;

        public void Damage(float damage, Vector3 knockback)
        {
            life -= damage * armor_factor;
            if (life <= 0)
            {
                Die();
            }
            // knockback
            unitRigidbody.AddForce(knockback);
        }
        
        

        void Die()
        {
            for (var i = 0; i < coins; i++)
            {
                var coin = Instantiate(ArenaController.Instance.coinPrefab, ArenaController.Instance.coinContainer);
                coin.transform.position = this.transform.position + new Vector3(0, 0.5f, 0);
                var rnd = ArenaController.Instance.rnd;
                coin.GetComponent<Rigidbody>().AddForce(
                    3 * new Vector3((float) rnd.NextDouble() * 2 - 1, (float) rnd.NextDouble(), (float) rnd.NextDouble() * 2 - 1));
            }
            Destroy(this.gameObject);
        }

        public void Start()
        {
        }

        // public void Update()
        // {
        //     unitRigidbody.angularVelocity = new Vector3(0, 0, 0);
        //     //unitRigidbody.constraints.
        //     //unitRigidbody.rotation.SetEulerAngles(new Vector3(0, 0, 0));
        //     unitRigidbody.velocity *= 0.99f;
        // }
        public void Walk(Vector3 speed, float strength)
        { 
           var dif = speed - unitRigidbody.velocity;
           var traction = 1 / (Vector3.Magnitude(dif) + 1); // the larger the speed difference, the lower the traction
           unitRigidbody.velocity += traction  * strength * dif;
        }
        public void FixedUpdate()
        {
            unitRigidbody.velocity *= 0.998f;
            if (turntransform is not null)
            {
                targetangle = 0;
                if (unitRigidbody.velocity.x < 0)
                    targetangle = 180;
                if (invertDirection)
                    targetangle = 180 - targetangle;
                rotationmomentum += 0.08f * (targetangle - currentangle);
                currentangle += rotationmomentum;
                rotationmomentum *= 0.80f;
                
                transform.rotation = Quaternion.Euler(0, currentangle, 0);
            }
            
            
        }
        
        void OnCollisionEnter(Collision collision)
        {
            var incomingRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (incomingRigidbody == null)
                return;
            GameObject obstracle = incomingRigidbody.gameObject;
            var player = obstracle.GetComponent<PlayerController>();
            if (player == null)
                return;
            player.Damage((incomingRigidbody.transform.position - this.transform.position).normalized * pushKnockback);
            // maybe play explosion to push away the enemies?
            // for now, we just remove the enemy so we don't get hurt again instantly. 
            if (player.invulnerableTimeLeft <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}