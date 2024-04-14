using System;
using UnityEditor.UI;
using UnityEngine;

namespace Components
{
    public class UnitController : MonoBehaviour
    {
        public Rigidbody unitRigidbody;
        public GameObject turnobject;
        public Transform turntransform;
        

        public float life = 100;
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

        public void Start()
        {
            if (turnobject is not null)
                turntransform = turnobject.GetComponent<Transform>();
        }

        // public void Update()
        // {
        //     unitRigidbody.angularVelocity = new Vector3(0, 0, 0);
        //     //unitRigidbody.constraints.
        //     //unitRigidbody.rotation.SetEulerAngles(new Vector3(0, 0, 0));
        //     unitRigidbody.velocity *= 0.99f;
        // }
        public void Update()
        {
            //if(turntransform is not null)
                if (unitRigidbody.velocity.x < 0)
                    turntransform.rotation.eulerAngles.Set(0,180,0);
                else
                    turntransform.rotation.eulerAngles.Set(0,0,0);
            
        }
    }
}