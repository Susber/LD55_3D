using System;
using UnityEditor.UI;
using UnityEngine;

namespace Components
{
    public class UnitController : MonoBehaviour
    {
        public Rigidbody unitRigidbody;

        public float life = 100;
        private float armor_factor = 1;
        private float armor;

        public int coins;

        public void Damage(float damage)
        {
            life -= damage * armor_factor;
            if (life <= 0)
            {
                Die();
            }
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

        // public void Start()
        // {
        //     unitRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        // }

        // public void Update()
        // {
        //     unitRigidbody.angularVelocity = new Vector3(0, 0, 0);
        //     //unitRigidbody.constraints.
        //     //unitRigidbody.rotation.SetEulerAngles(new Vector3(0, 0, 0));
        //     unitRigidbody.velocity *= 0.99f;
        // }
    }
}