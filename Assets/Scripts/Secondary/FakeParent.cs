using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    //The script basically establishes a parent-child relationship between another object and this object. Used to parent things to tree branches without it being confusing
    //Note: This only works for position, as I didn't have the time to sort out rotation.
    public class FakeParent : MonoBehaviour
    {
        public GameObject targetParent;

        Vector3 positionOffset;
        
        void Start()
        {
            positionOffset = transform.position - targetParent.transform.position; 
        }
        void Update()
        {
            transform.position = targetParent.transform.position + positionOffset;
        }
    }
}