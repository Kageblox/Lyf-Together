using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    //Simply rotates the gameObject attached to the script by rotationPerSecond every second.
    public class Rotator : MonoBehaviour
    {
        public Vector3 rotationPerSecond;
        void Update()
        {
            transform.rotation *= Quaternion.Euler(rotationPerSecond * Time.deltaTime);
        }
    }
}