using UnityEngine;

namespace StompyBlondie
{
    public class Spinner: MonoBehaviour
    {
        public enum SpinDirection
        {
            Clockwise,
            CounterClockwise
        };

        public SpinDirection direction;
        public float speed = 50f;

        public void Update()
        {
            if(direction == SpinDirection.CounterClockwise)
                transform.Rotate(Vector3.forward * Time.deltaTime * speed);
            else
                transform.Rotate(-(Vector3.forward * Time.deltaTime * speed));
        }
    }
}