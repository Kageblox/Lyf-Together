using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    //This script saves its children lights into presets to load.
    public class LightingGroup : MonoBehaviour
    {
        public Primary.TimeLighting.LightPreset[] presets = new Primary.TimeLighting.LightPreset[3];
        void Awake()
        {
            Light[] lights = transform.GetComponentsInChildren<Light>();
            for (int i = 0; i < 3; i++)
            {
                presets[i].SavePreset(lights[i]);
                lights[i].gameObject.SetActive(false);
            }
        }
    }
}