using UnityEngine;

namespace Collisions
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        private void Update()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            transform.position += new Vector3(x, y, 0) * (_speed * Time.deltaTime);
        }
    }
}