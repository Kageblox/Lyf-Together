using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Primary
{
    public class TimeSwapper : MonoBehaviour
    {
        [Serializable]
        public class TimingPreset
        {
            public Sprite backgroundSprite;
            public Material leavesMaterial;
            public Secondary.LightingGroup lightingGroup;
        }
        public SpriteRenderer backgroundRenderer;
        public SpriteRenderer previousBackgroundRenderer;
        public SkinnedMeshRenderer leavesRenderer;
        public TimeLighting timeLighting;

        public float interpolationDuration;
        IEnumerator runningInterpolationEnu;

        public TimingPreset morningTimePresets;
        public TimingPreset afternoonTimePresets;
        public TimingPreset eveningTimePresets;
        public TimingPreset nightTimePresets;
        //0 - Morning
        //1 - Afternoon
        //2 - Evening
        //3 - Night
        void Start()
        {
            int currentTime = DateTime.Now.Hour;
            
            /*
            //Morning is 7 to 12
            if (currentTime >= 7 && currentTime < 12)
            {
                LoadPresetInstantly(morningTimePresets);
            }
            //Afternoon is 12 to 5
            else if (currentTime >= 12 && currentTime < 17)
            {
                LoadPresetInstantly(afternoonTimePresets);
            }
            //Evening is 5 to 7
            else if (currentTime >= 17 && currentTime < 19)
            {
                LoadPresetInstantly(eveningTimePresets);
            }
            //Night is every other timing
            else
            {
                LoadPresetInstantly(nightTimePresets);
            }
            */

        }
        public void ChangeTime(int time)
        {
            switch (time)
            {
                case 0:
                    InterpolateToPreset(morningTimePresets);
                    break;
                case 1:
                    InterpolateToPreset(afternoonTimePresets);
                    break;
                case 2:
                    InterpolateToPreset(eveningTimePresets);
                    break;
                case 3:
                    InterpolateToPreset(nightTimePresets);
                    break;
            }
        }
        public void LoadPresetInstantly(TimingPreset preset)
        {
            backgroundRenderer.sprite = preset.backgroundSprite;
            previousBackgroundRenderer.sprite = preset.backgroundSprite;

            leavesRenderer.material = preset.leavesMaterial;
            timeLighting.LoadPresetInstantly(preset.lightingGroup);
        }
        public void InterpolateToPreset(TimingPreset preset)
        {
            timeLighting.InterpolateToLightPreset(preset.lightingGroup, interpolationDuration);

            if (runningInterpolationEnu != null)
            {
                StopCoroutine(runningInterpolationEnu);
            }
            runningInterpolationEnu = InterpolationEnu(preset);
            StartCoroutine(runningInterpolationEnu);
        }
        IEnumerator InterpolationEnu(TimingPreset timingPreset)
        {
            previousBackgroundRenderer.sprite = backgroundRenderer.sprite;
            backgroundRenderer.sprite = timingPreset.backgroundSprite;
            backgroundRenderer.color = new Color(1,1,1,0);

            float t = 0;
            while (t < interpolationDuration)
            {
                t += Time.deltaTime;

                backgroundRenderer.color = Color.Lerp(backgroundRenderer.color, Color.white, (3f / interpolationDuration) * Time.deltaTime);

                Material currentMat = leavesRenderer.material;
                currentMat.Lerp(leavesRenderer.material, timingPreset.leavesMaterial, (3f / interpolationDuration) * Time.deltaTime);
                leavesRenderer.material = currentMat;
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        /*
        public void LoadPreset(TimingPreset preset)
        {
            if (currentLighting != null)
            {
                currentLighting.SetActive(false);
            }
            backgroundRenderer.sprite = preset.backgroundSprite;
            leavesRenderer.material = preset.leavesMaterial;
            currentLighting = preset.lighting;
            currentLighting.SetActive(true);
        }
        */
    }
}