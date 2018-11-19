using UnityEngine;
using System.Collections;

namespace OSP
{
    public class PlayerMovement : MonoBehaviour
    {
        public bool AllowXMovement = false;
        public bool AllowYMovement = false;

        void Update()
        {
            float moveX = Input.GetAxis("Horizontal");
            if (!AllowXMovement)
                moveX = 0;

            float moveY = Input.GetAxis("Vertical");
            if (!AllowYMovement)
                moveY = 0;

            transform.position = new Vector3(transform.position.x + moveX / 10, transform.position.y + moveY / 10, transform.position.z);
        }
    }
}