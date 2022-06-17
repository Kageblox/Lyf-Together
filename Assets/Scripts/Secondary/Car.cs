using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    //This script controls Car game models.
    public class Car : MonoBehaviour
    {
        #region Other Components
        [Header("Other Components")]
        public MeshRenderer carRenderer;
        public Material dayWindows;
        public Material nightWindows;
        public GameObject headlights;

        [Header("Variables")]
        //Material arrays used to change the material of the car, as single materials can't be changed out, but instead an entire new array needs to be slotted in.
        Material[] dayMaterialArray;
        Material[] nightMaterialArray;
        #endregion

        #region MonoBehavior Functions
        void Awake()
        {
            //First, define the day and night material arrays. Material slot [2] refers to the windows of the car, so by changing it, I can define the Day and Night material arrays
            dayMaterialArray = carRenderer.materials;
            dayMaterialArray[2] = dayWindows;

            nightMaterialArray = carRenderer.materials;
            nightMaterialArray[2] = nightWindows;
        }
        #endregion

        #region Functions and Coroutines
        //Simply enables/disables the headlights and changes the materials based on whether Night mode is supposed to be active
        public void SetNightMode(bool val)
        {
            headlights.SetActive(val);
            if (val)
            {
                carRenderer.materials = nightMaterialArray;
            }
            else
            {
                carRenderer.materials = dayMaterialArray;
            }
        }
        #endregion
    }
}