using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    public class LightingGroup : MonoBehaviour
    {
        public Primary.TimeLighting.LightPreset[] presets = new Primary.TimeLighting.LightPreset[3];
        void Start()
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