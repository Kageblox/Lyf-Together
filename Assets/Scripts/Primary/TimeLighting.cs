using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Secondary;

namespace Primary
{
    public class TimeLighting : MonoBehaviour
    {
        [Serializable]
        public class LightPreset
        {
            public Color color;
            public float intensity;
            public Quaternion rotation;
            public LightPreset(Color new_Color, float new_Intensity, Quaternion new_Rotation)
            {
                color = new_Color;
                intensity = new_Intensity;
                rotation = new_Rotation;
            }
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
            public void InterpolateTowardsPreset(ref Light light, float interpolation)
            {
                light.color = Color.Lerp(light.color, color, interpolation);
                light.intensity = Mathf.Lerp(light.intensity, intensity, interpolation);
                light.transform.rotation = Quaternion.Lerp(light.transform.rotation, rotation, interpolation);
            }
            public void InterpolateTowardsPreset(ref LightPreset light, float interpolation)
            {
                light.color = Color.Lerp(light.color, color, interpolation);
                light.intensity = Mathf.Lerp(light.intensity, intensity, interpolation);
                light.rotation = Quaternion.Lerp(light.rotation, rotation, interpolation);
            }
        }

        Light[] lights;
        IEnumerator runningInterpolationEnu;
        void Start()
        {
            lights = transform.GetComponentsInChildren<Light>();
        }
        public void LoadPresetInstantly(LightingGroup lightPresetGroup)
        {
            lightPresetGroup.presets[0].LoadPreset(ref lights[0]);
            lightPresetGroup.presets[1].LoadPreset(ref lights[1]);
            lightPresetGroup.presets[2].LoadPreset(ref lights[2]);
        }
        public void InterpolateToLightPreset(LightingGroup lightPresetGroup, float interpolationDuration)
        {
            if (runningInterpolationEnu!= null)
            {
                StopCoroutine(runningInterpolationEnu);
            }
            runningInterpolationEnu = InterpolateToLightPresetEnu(lightPresetGroup, interpolationDuration);
            StartCoroutine(runningInterpolationEnu);
        }
        IEnumerator InterpolateToLightPresetEnu(LightingGroup lightPresetGroup, float interpolationDuration)
        {
            LightPreset[] currentPreset = new LightPreset[3];
            for (int i = 0; i < lights.Length; i++)
            {
                currentPreset[i] = new LightPreset(lights[i].color, lights[i].intensity, lights[i].transform.rotation);
            }
            
            float t = 0;
            while (t < interpolationDuration)
            {
                t += Time.deltaTime;
                float interpolation = t / interpolationDuration;
                Debug.Log(interpolation);
                for (int i = 0; i < lights.Length; i++)
                {
                    lightPresetGroup.presets[i].InterpolateTowardsPreset(ref lights[i], (3f/interpolationDuration) * Time.deltaTime);
                    //currentPreset[i].LoadPreset(ref lights[i]);
                }
                yield return new WaitForEndOfFrame();

            }
            yield break;
        }
    }

}