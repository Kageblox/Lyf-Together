using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Primary
{
    //This script is in charge of swapping between the different timings available. It does this by loading different presets, which will be further explained below
    public class TimeSwapper : MonoBehaviour
    {
        #region Classes
        //A preset containing references and variables which determine how a timing will look and work.
        [Serializable]
        public class TimingPreset
        {
            //The sprite in the background.
            public Sprite backgroundSprite;

            //The intended material of leaves to load
            public Material leavesMaterial;

            //The lighting group data to load. Further explained in LightingGroup (Found in Secondary)
            public Secondary.LightingGroup lightingGroup;

            //The props to enable when loading this scene. Not limited to props, but can also be function calls and such. Further explained in PropsGroup (Found in Secondary)
            public Secondary.EventsGroup events;

            //Whether the headlights of cars is supposed to be on in the scene
            public bool carLightsOn;
        }
        #endregion

        #region Variables
        [Header("Other Components")]
        //The true background renderer. Renderers over the previous background renderer
        public SpriteRenderer backgroundRenderer;

        //The previous background renderer. Meant to show the previous preset's background while the true background renderer renders over it
        public SpriteRenderer previousBackgroundRenderer;

        //The mesh renderer for the leaves. The material will change depending on the leavesMaterial slot of the current preset
        public SkinnedMeshRenderer leavesRenderer;

        //The script that controls the lighting of the scene.
        public TimeLighting timeLighting;

        [Header("Variables")]
        //The available presets to load. Add more presets here if more presets are required
        public TimingPreset morningTimePresets;
        public TimingPreset afternoonTimePresets;
        public TimingPreset eveningTimePresets;
        public TimingPreset nightTimePresets;

        //The current preset being loaded
        TimingPreset currentPreset;

        //The current running interpolation coroutine
        IEnumerator runningInterpolationEnu;

        [Header("Settings")]
        //The time needed to interpolate between different presets
        public float interpolationDuration;
        #endregion

        #region MonoBehavior Functions
        void Start()
        {
            //First retrieve the current time.
            int currentTime = DateTime.Now.Hour;

            //Then load the preset that best fits the current timing
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
        }
        #endregion

        #region Functions and Coroutines
        //Interpolates to the next preset. Depends on the given ID (Explained below)
        //0 = Morning
        //1 = Afternoon
        //2 = Evening
        //3 = Night
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

        //Instantly loads a preset.
        public void LoadPresetInstantly(TimingPreset preset)
        {
            //If there was a previous preset, exit it
            if (currentPreset != null)
            {
                currentPreset.events.onExit();
            }

            //Next, change the currentPreset variable, to help keep track of what the current preset is, and trigger its onEnter function.
            currentPreset = preset;
            preset.events.OnEnter();

            //Since there's no interpolation, we simply change the sprite for both backgrounds
            backgroundRenderer.sprite = preset.backgroundSprite;
            previousBackgroundRenderer.sprite = preset.backgroundSprite;

            //We can also instantly load the new material of the leaves
            leavesRenderer.material = preset.leavesMaterial;
            
            //And also instantly load the current lighting
            timeLighting.LoadPresetInstantly(preset.lightingGroup);
        }

        //Interpolates the scene to the next preset
        public void InterpolateToPreset(TimingPreset preset)
        {
            //First, activate the interpolation function on timeLighting, as it has its own coroutine due to its complexity
            timeLighting.InterpolateToLightPreset(preset.lightingGroup, interpolationDuration);

            //Next, if there's currently an interpolation coroutine running, stop it
            if (runningInterpolationEnu != null)
            {
                StopCoroutine(runningInterpolationEnu);
            }

            //Finally, start the coroutine
            runningInterpolationEnu = InterpolationEnu(preset);
            StartCoroutine(runningInterpolationEnu);
        }

        IEnumerator InterpolationEnu(TimingPreset timingPreset)
        {
            //Start off by setting previousBackgroundRenderer to the previous background sprite.
            previousBackgroundRenderer.sprite = backgroundRenderer.sprite;

            //Next, set the true background renderer to the target background sprite. But make it invisible, as it will slowly fade into being solid over the interpolation duration.
            backgroundRenderer.sprite = timingPreset.backgroundSprite;
            backgroundRenderer.color = new Color(1,1,1,0);

            //After that, trigger the OnExit function on the events of the previous preset
            currentPreset.events.onExit();

            //Now, for as long as interpolation duration lasts,
            float t = 0;
            while (t < interpolationDuration)
            {
                t += Time.deltaTime;

                //I am honestly not sure why this formula works, but it seems to work the best.
                float lerp = (3f / interpolationDuration) * Time.deltaTime;

                //But using the lerp value, lerp the transparency of the background to a solid white (Which just means the actual texture)
                backgroundRenderer.color = Color.Lerp(backgroundRenderer.color, Color.white, lerp);

                //After that, I retrieve the current material of the leaves
                Material currentMat = leavesRenderer.material;

                //And interpolate from that to the target leaves material
                currentMat.Lerp(currentMat, timingPreset.leavesMaterial, lerp);

                //Before finally applying the material
                leavesRenderer.material = currentMat;

                //And waiting for the next material
                yield return new WaitForEndOfFrame();
            }
            //Finally, update the currentPreset variable, and trigger its event's onEnter functions, before finally ending the coroutine
            currentPreset = timingPreset;
            currentPreset.events.OnEnter();
            yield break;
        }
        #endregion
    }
}