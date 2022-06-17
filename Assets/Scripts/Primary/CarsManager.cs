using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Secondary;

namespace Primary
{
    //This script is in charge of managing all cars in the scene.
    public class CarsManager : MonoBehaviour
    {
        #region Variables
        [Header("Variables")]
        //A collection of all cars in the scene
        Car[] cars;
        #endregion

        #region MonoBehavior Functions
        void Awake()
        {
            //At the start, retrieve all car instances in the scene
            cars = FindObjectsOfType<Car>();
        }
        #endregion

        #region Functions and Coroutines
        //Enables or disables Night mode on all cars in the scene
        public void SetAllCarsNightMode(bool val)
        {
            foreach (Car car in cars)
            {
                car.SetNightMode(val);
            }
        }
        #endregion
    }
}
