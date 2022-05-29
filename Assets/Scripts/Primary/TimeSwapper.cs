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
            public GameObject lighting;
        }
        public SpriteRenderer backgroundRenderer;
        public SkinnedMeshRenderer leavesRenderer;

        public TimingPreset morningTimePresets;
        public TimingPreset afternoonTimePresets;
        public TimingPreset eveningTimePresets;
        public TimingPreset nightTimePresets;

        GameObject currentLighting;
        //0 - Morning
        //1 - Afternoon
        //2 - Evening
        //3 - Night
        void Start()
        {
            int currentTime = DateTime.Now.Hour;
            
            //Morning is 7 to 12
            if (currentTime >= 7 && currentTime < 12)
            {
                LoadPreset(morningTimePresets);
            }
            //Afternoon is 12 to 5
            else if (currentTime >= 12 && currentTime < 17)
            {
                LoadPreset(afternoonTimePresets);
            }
            //Evening is 5 to 7
            else if (currentTime >= 17 && currentTime < 19)
            {
                LoadPreset(eveningTimePresets);
            }
            //Night is every other timing
            else
            {
                LoadPreset(nightTimePresets);
            }

        }
        public void ChangeTime(int time)
        {
            switch (time)
            {
                case 0:
                    LoadPreset(morningTimePresets);
                    break;
                case 1:
                    LoadPreset(afternoonTimePresets);
                    break;
                case 2:
                    LoadPreset(eveningTimePresets);
                    break;
                case 3:
                    LoadPreset(nightTimePresets);
                    break;
            }
        }
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
    }
}