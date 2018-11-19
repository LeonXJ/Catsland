using UnityEngine;
using System.Collections;

namespace OSP
{
    /// <summary>
    /// Class AutoPlayerScroll. Just a simple script to automatically move the player.
    /// </summary>
    public class AutoPlayerScroll : MonoBehaviour
    {
        /// <summary>
        /// The horizontal speed of the player.
        /// </summary>
        public float SpeedX = 0;
        /// <summary>
        /// The vertical speed of the player.
        /// </summary>
        public float SpeedY = 0;

        /// <summary>
        /// Moves the player at the given speed(s) every frame.
        /// </summary>
        void Update()
        {
            transform.position = new Vector3(transform.position.x + SpeedX / 10, transform.position.y + SpeedY / 10, transform.position.z);
        }
    }
}