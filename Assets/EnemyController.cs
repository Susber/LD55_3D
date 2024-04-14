using UnityEngine;

namespace Components
{
    public class EnemyController : UnitController
    {
        public float speed = 5.0f;
        
        
        new public void Start(){
            base.Start();
        }
        new void Update()
        {
            base.Update();
            var from = this.transform.position;
            var to = PlayerController.Instance.transform.position;
            if ((to - from).sqrMagnitude < 1)
            {
                return;
            }
            var dir = (to - from).normalized;
            this.Push(speed * dir);
        }
    }
}