using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Primary
{
    //This class manages the lights in the current scene.
    public class TimeLighting : MonoBehaviour
    {
        #region Classes
        //Contains data on Light components, and has functions to save and load these functions on actual Light components
        [Serializable]
        public class LightPreset
        {
            //Variables that define the Light
            public Color color;
            public float intensity;
            public Quaternion rotation;

            //The constructor of Light Preset
            public LightPreset(Color new_Color, float new_Intensity, Quaternion new_Rotation)
            {
                color = new_Color;
                intensity = new_Intensity;
                rotation = new_Rotation;
            }

            //Save and load functions for Light Preset
            public void SavePreset(Light light)
            {
                color = light.color;
                intensity = light.intensity;
                rotation = light.transform.rotation;
            }
            public void LoadPreset(ref Light light)
            {
                light.color = color;
                light.intensity = intensity;
                light.transform.rotation = rotation;
            }

            //Function to interpolate a Light component to the preset
            public void InterpolateTowardsPreset(ref Light light, float interpolation)
            {
                light.color = Color.Lerp(light.color, color, interpolation);
                light.intensity = Mathf.Lerp(light.intensity, intensity, interpolation);
                light.transform.rotation = Quaternion.Lerp(light.transform.rotation, rotation, interpolation);
            }
        }
        #endregion

        #region Variables
        [Header("Variables")]
        //An array that contains the Light components of its children. As loading it in start leads to errors, please slot the lights in instead
        public Light[] lights;
        //The currently running interpolation coroutine
        IEnumerator runningInterpolationEnu;
        #endregion

        #region Functions and Coroutines
        //Instantly loads a lighting group, which is a collection of 3 presets, onto the 3 lights actually responsible for the scene's lighting
        //Note: Please see LightingGroup in Secondary for more info.
        public void LoadPresetInstantly(Secondary.LightingGroup lightPresetGroup)
        {
            lightPresetGroup.presets[0].LoadPreset(ref lights[0]);
            lightPresetGroup.presets[1].LoadPreset(ref lights[1]);
            lightPresetGroup.presets[2].LoadPreset(ref lights[2]);
        }

        //Initates the interpolation process to a target lighting group
        public void InterpolateToLightPreset(Secondary.LightingGroup lightPresetGroup, float interpolationDuration)
        {
            //First, if an interpolation is currently running, stop it 
            if (runningInterpolationEnu!= null)
            {
                StopCoroutine(runningInterpolationEnu);
            }

            //Before defining a new one and running it
            runningInterpolationEnu = InterpolateToLightPresetEnu(lightPresetGroup, interpolationDuration);
            StartCoroutine(runningInterpolationEnu);
        }

        IEnumerator InterpolateToLightPresetEnu(Secondary.LightingGroup lightPresetGroup, float interpolationDuration)
        {
            //For the defined interpolationDuration
            float t = 0;
            while (t < interpolationDuration)
            {
                t += Time.deltaTime;
                for (int i = 0; i < lights.Length; i++)
                {
                    //Honestly don't know why this formula works, but it does
                    float lerp = (3f / interpolationDuration) * Time.deltaTime;

                    //Interpolate each of the lights to their respective preset present inside the target lighting group.
                    lightPresetGroup.presets[i].InterpolateTowardsPreset(ref lights[i], lerp);
                }
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        #endregion
    }
}