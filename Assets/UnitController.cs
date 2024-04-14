using System;
using UnityEditor.UI;
using UnityEngine;

namespace Components
{
    public class UnitController : MonoBehaviour
    {
        public Rigidbody unitRigidbody;

        private float life = 100;
        private float armor_factor = 1;
        private float armor;

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