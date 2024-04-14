using UnityEngine;


    public class Util
    {
        
        public static Vector2 Rotate2d(Vector2 v, float angle)
        {
            var c = Mathf.Cos(angle);
            var s = Mathf.Sin(angle);
            return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
        }
    }
